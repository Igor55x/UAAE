using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityTools.Utils;
using UnityTools.Compression;

namespace UnityTools
{
    public class AssetBundleFile
    {
        public AssetBundleHeader Header;
        public AssetBundleMetadata Metadata;

        public AssetsFileReader Reader;

        public void Close() => Reader.Close();

        public bool Read(AssetsFileReader reader, bool allowCompressed = false)
        {
            Reader = reader;
            Header = new AssetBundleHeader();
            Header.Read(Reader);
            Metadata = new AssetBundleMetadata();

            if (Header.GetCompressionType() != 0)
            {
                if (allowCompressed)
                    return true;

                Close();
                return false;
            }
            reader.Position = Header.GetBundleInfoOffset();
            Metadata.Read(Header, reader);
            return true;
        }

        public bool Write(AssetsFileWriter writer, List<BundleReplacer> replacers, ClassDatabaseFile typeMeta = null)
        {
            Header.Write(writer);
            var newMetadata = new AssetBundleMetadata
            {
                Hash = new Hash128(new byte[16]),
                //I could map the assets to their blocks but I don't
                //have any more-than-1-block files to test on
                //this should work just fine as far as I know
                BlocksInfo = new[]
                {
                    new AssetBundleBlockInfo
                    {
                        CompressedSize = 0,
                        DecompressedSize = 0,
                        Flags = 0x40
                    }
                }
            };

            //assets that did not have their data modified but need
            //the original info to read from the original file
            var newToOriginalDirInfoLookup = new Dictionary<AssetBundleDirectoryInfo, AssetBundleDirectoryInfo>();
            var originalDirInfos = new List<AssetBundleDirectoryInfo>();
            var dirInfos = new List<AssetBundleDirectoryInfo>();
            var currentReplacers = replacers.ToList();
            //this is kind of useless at the moment but leaving it here
            //because if the AssetsFile size can be precalculated in the
            //future, we can use this to skip rewriting sizes
            long currentOffset = 0;

            //write all original files, modify sizes if needed and skip those to be removed
            for (var i = 0; i < Metadata.DirectoryCount; i++)
            {
                var info = Metadata.DirectoryInfo[i];
                originalDirInfos.Add(info);
                var newInfo = new AssetBundleDirectoryInfo
                {
                    Offset = currentOffset,
                    DecompressedSize = info.DecompressedSize,
                    Flags = info.Flags,
                    Name = info.Name
                };

                var replacer = currentReplacers.FirstOrDefault(n => n.GetOriginalEntryName() == newInfo.Name);
                if (replacer != null)
                {
                    currentReplacers.Remove(replacer);
                    if (replacer.GetReplacementType() == BundleReplacementType.AddOrModify)
                    {
                        newInfo = new AssetBundleDirectoryInfo
                        {
                            Offset = currentOffset,
                            DecompressedSize = replacer.GetSize(),
                            Flags = info.Flags,
                            Name = replacer.GetEntryName()
                        };
                    }
                    else if (replacer.GetReplacementType() == BundleReplacementType.Rename)
                    {
                        newInfo = new AssetBundleDirectoryInfo
                        {
                            Offset = currentOffset,
                            DecompressedSize = info.DecompressedSize,
                            Flags = info.Flags,
                            Name = replacer.GetEntryName()
                        };
                        newToOriginalDirInfoLookup[newInfo] = info;
                    }

                    else if (replacer.GetReplacementType() == BundleReplacementType.Remove)
                    {
                        continue;
                    }
                }
                else
                {
                    newToOriginalDirInfoLookup[newInfo] = info;
                }

                if (newInfo.DecompressedSize != -1)
                {
                    currentOffset += newInfo.DecompressedSize;
                }

                dirInfos.Add(newInfo);
            }

            //write new files
            while (currentReplacers.Count > 0)
            {
                var replacer = currentReplacers[0];
                if (replacer.GetReplacementType() == BundleReplacementType.AddOrModify)
                {
                    var info = new AssetBundleDirectoryInfo
                    {
                        Offset = currentOffset,
                        DecompressedSize = replacer.GetSize(),
                        Flags = replacer.HasSerializedData() ? 4U : 0U,
                        Name = replacer.GetEntryName()
                    };
                    currentOffset += info.DecompressedSize;

                    dirInfos.Add(info);
                }
                currentReplacers.Remove(replacer);
            }

            //write the listings
            var bundleInfPos = writer.Position;
            newMetadata.DirectoryInfo = dirInfos.ToArray(); //this is only here to allocate enough space so it's fine if it's inaccurate
            newMetadata.Write(Header, writer);

            var assetDataPos = writer.Position;

            //actually write the file data to the bundle now
            foreach (var dirInfo in dirInfos)
            {
                var info = dirInfo;
                var replacer = replacers.FirstOrDefault(n => n.GetEntryName() == info.Name);
                if (replacer != null)
                {
                    if (replacer.GetReplacementType() == BundleReplacementType.AddOrModify)
                    {
                        var startPos = writer.Position;
                        var endPos = replacer.Write(writer);
                        var size = endPos - startPos;

                        dirInfo.DecompressedSize = size;
                        dirInfo.Offset = startPos - assetDataPos;
                    }
                }
                else
                {
                    if (newToOriginalDirInfoLookup.TryGetValue(info, out var originalInfo))
                    {
                        var startPos = writer.Position;

                        Reader.Position = Header.GetFileDataOffset() + originalInfo.Offset;
                        Reader.BaseStream.CopyToCompat(writer.BaseStream, (int)originalInfo.DecompressedSize);

                        dirInfo.Offset = startPos - assetDataPos;
                    }
                }
            }

            //now that we know what the sizes are of the written files let's go back and fix them
            var finalSize = writer.Position;
            var assetSize = (uint)(finalSize - assetDataPos);

            writer.Position = bundleInfPos;
            newMetadata.BlocksInfo[0].DecompressedSize = assetSize;
            newMetadata.BlocksInfo[0].CompressedSize = assetSize;
            newMetadata.DirectoryInfo = dirInfos.ToArray();
            newMetadata.Write(Header, writer);

            var infoSize = (uint)(assetDataPos - bundleInfPos);

            writer.Position = 0;
            var newHeader = new AssetBundleHeader
            {
                Signature = Header.Signature,
                Version = Header.Version,
                MinUnityVersion = Header.MinUnityVersion,
                UnityVersion = Header.UnityVersion,
                Size = finalSize,
                CompressedSize = infoSize,
                DecompressedSize = infoSize,
                Flags = Header.Flags & unchecked((uint)~0x80) & unchecked((uint)~0x3f) //unset info at end flag and compression value
            };
            newHeader.Write(writer);
            return true;
        }

        public bool Unpack(AssetsFileReader reader, AssetsFileWriter writer)
        {
            reader.Position = 0;
            if (Read(reader, true))
            {
                reader.Position = Header.GetBundleInfoOffset();
                var blocksInfoStream = new MemoryStream();
                var compressedSize = (int)Header.CompressedSize;
                var blocksInfoBytes = reader.ReadBytes(compressedSize);

                switch (Header.GetCompressionType())
                {
                    case AssetBundleCompressionType.Lzma:
                        {
                            using var ms = new MemoryStream(blocksInfoBytes);
                            LzmaHelper.DecompressStream(ms, blocksInfoStream);
                            break;
                        }
                    case AssetBundleCompressionType.Lz4:
                    case AssetBundleCompressionType.Lz4HC:
                        {
                            var uncompressedSize = (int)Header.DecompressedSize;
                            var uncompressedBytes = Lz4Helper.Decompress(blocksInfoBytes, uncompressedSize);
                            blocksInfoStream = new MemoryStream(uncompressedBytes);
                            break;
                        }
                    default:
                        blocksInfoStream = null;
                        break;
                }
                if (Header.GetCompressionType() != 0)
                {
                    using var memReader = new AssetsFileReader(blocksInfoStream)
                    {
                        Position = 0
                    };
                    Metadata.Read(Header, memReader);
                }
                var newBundleHeader6 = new AssetBundleHeader
                {
                    Signature = Header.Signature,
                    Version = Header.Version,
                    MinUnityVersion = Header.MinUnityVersion,
                    UnityVersion = Header.UnityVersion,
                    Size = 0,
                    CompressedSize = Header.DecompressedSize,
                    DecompressedSize = Header.DecompressedSize,
                    Flags = Header.Flags & 0x40 //set compression and block position to 0
                };
                var fileSize = newBundleHeader6.GetFileDataOffset();
                for (var i = 0; i < Metadata.BlockCount; i++)
                    fileSize += Metadata.BlocksInfo[i].DecompressedSize;
                newBundleHeader6.Size = fileSize;
                var newBundleInf6 = new AssetBundleMetadata()
                {
                    Hash = new Hash128(new byte[16]), //-todo, figure out how to make real hash, uabe sets these to 0 too
                    BlockCount = Metadata.BlockCount,
                    DirectoryCount = Metadata.DirectoryCount
                };
                newBundleInf6.BlocksInfo = new AssetBundleBlockInfo[newBundleInf6.BlockCount];
                for (var i = 0; i < newBundleInf6.BlockCount; i++)
                {
                    newBundleInf6.BlocksInfo[i] = new AssetBundleBlockInfo
                    {
                        CompressedSize = Metadata.BlocksInfo[i].DecompressedSize,
                        DecompressedSize = Metadata.BlocksInfo[i].DecompressedSize,
                        Flags = (ushort)(Metadata.BlocksInfo[i].Flags & 0xC0) //set compression to none
                    };
                }
                newBundleInf6.DirectoryInfo = new AssetBundleDirectoryInfo[newBundleInf6.DirectoryCount];
                for (var i = 0; i < newBundleInf6.DirectoryCount; i++)
                {
                    newBundleInf6.DirectoryInfo[i] = new AssetBundleDirectoryInfo
                    {
                        Offset = Metadata.DirectoryInfo[i].Offset,
                        DecompressedSize = Metadata.DirectoryInfo[i].DecompressedSize,
                        Flags = Metadata.DirectoryInfo[i].Flags,
                        Name = Metadata.DirectoryInfo[i].Name
                    };
                }
                newBundleHeader6.Write(writer);
                if (newBundleHeader6.Version >= 7)
                {
                    writer.Align16();
                }
                newBundleInf6.Write(Header, writer);

                reader.Position = Header.GetFileDataOffset();
                for (var i = 0; i < newBundleInf6.BlockCount; i++)
                {
                    var info = Metadata.BlocksInfo[i];
                    var compressedBlockSize = (int)info.CompressedSize;
                    var decompressedBlockSize = (int)info.DecompressedSize;
                    var compressedBlock = reader.ReadBytes(compressedBlockSize);
                    switch (info.GetCompressionType())
                    {
                        case 0:
                            writer.Write(compressedBlock);
                            break;
                        case 1:
                            LzmaHelper.DecompressStream(reader.BaseStream, writer.BaseStream, compressedBlockSize, decompressedBlockSize);
                            break;
                        case 2:
                        case 3:
                            var decompressedBlock = Lz4Helper.Decompress(compressedBlock, decompressedBlockSize);
                            writer.Write(decompressedBlock);
                            break;
                        }
                }
                return true;
            }
            return false;
        }

        public bool Pack(AssetsFileReader reader, AssetsFileWriter writer, AssetBundleCompressionType compType)
        {
            reader.Position = 0;
            writer.Position = 0;
            if (!Read(reader))
                return false;

            var blockInfoAtEnd = Header.IsBlocksInfoAtTheEnd();
            var newHeader = new AssetBundleHeader
            {
                Signature = Header.Signature,
                Version = Header.Version,
                MinUnityVersion = Header.MinUnityVersion,
                UnityVersion = Header.UnityVersion,
                Size = 0,
                CompressedSize = 0,
                DecompressedSize = 0,
                Flags = (uint)(0x43 | (blockInfoAtEnd ? 0x80 : 0x00))
            };

            var newMetadata = new AssetBundleMetadata
            {
                Hash = new Hash128(new byte[16]),
                BlockCount = 0,
                BlocksInfo = null,
                DirectoryCount = Metadata.DirectoryCount,
                DirectoryInfo = Metadata.DirectoryInfo
            };

            // Write header now and overwrite it later
            var startPos = writer.Position;

            newHeader.Write(writer);
            if (newHeader.Version >= 7)
                writer.Align16();

            var headerSize = (int)(writer.Position - startPos);

            var totalCompressedSize = 0L;
            var newBlocks = new List<AssetBundleBlockInfo>();
            var newStreams = new List<Stream>(); // Used if blockInfoAtEnd == false

            var fileDataOffset = Header.GetFileDataOffset();
            var fileDataLength = (int)(Header.Size - fileDataOffset);

            var bundleDataStream = new SegmentStream(reader.BaseStream, fileDataOffset, fileDataLength);

            switch (compType)
            {
                case AssetBundleCompressionType.Lzma:
                    {
                        Stream writeStream;
                        if (blockInfoAtEnd)
                            writeStream = writer.BaseStream;
                        else
                            writeStream = GetTempFileStream();

                        var writeStreamStart = writeStream.Position;
                        LzmaHelper.CompressStream(bundleDataStream, writeStream);
                        var writeStreamLength = (uint)(writeStream.Position - writeStreamStart);

                        var blockInfo = new AssetBundleBlockInfo
                        {
                            CompressedSize = writeStreamLength,
                            DecompressedSize = (uint)fileDataLength,
                            Flags = 0x41
                        };

                        totalCompressedSize += blockInfo.CompressedSize;
                        newBlocks.Add(blockInfo);

                        if (!blockInfoAtEnd)
                            newStreams.Add(writeStream);
                        break;
                    }
                case AssetBundleCompressionType.Lz4:
                    {
                        // Compress into 0x20000 blocks
                        var bundleDataReader = new BinaryReader(bundleDataStream);
                        var uncompressedBlock = bundleDataReader.ReadBytes(0x20000);
                        while (uncompressedBlock.Length != 0)
                        {
                            Stream writeStream;
                            if (blockInfoAtEnd)
                                writeStream = writer.BaseStream;
                            else
                                writeStream = GetTempFileStream();

                            var compressedBlock = Lz4Helper.Compress(uncompressedBlock);
                            if (compressedBlock.Length > uncompressedBlock.Length)
                            {
                                writeStream.Write(uncompressedBlock, 0, uncompressedBlock.Length);
                                
                                var blockInfo = new AssetBundleBlockInfo
                                {
                                    CompressedSize = (uint)uncompressedBlock.Length,
                                    DecompressedSize = (uint)uncompressedBlock.Length,
                                    Flags = 0x00
                                };
                                
                                totalCompressedSize += blockInfo.CompressedSize;
                                newBlocks.Add(blockInfo);
                            }
                            else
                            {
                                writeStream.Write(compressedBlock, 0, compressedBlock.Length);

                                var blockInfo = new AssetBundleBlockInfo
                                {
                                    CompressedSize = (uint)compressedBlock.Length,
                                    DecompressedSize = (uint)uncompressedBlock.Length,
                                    Flags = 0x03
                                };

                                totalCompressedSize += blockInfo.CompressedSize;

                                newBlocks.Add(blockInfo);
                            }

                            if (!blockInfoAtEnd)
                                newStreams.Add(writeStream);

                            uncompressedBlock = bundleDataReader.ReadBytes(0x20000);
                        }
                        break;
                    }
                case AssetBundleCompressionType.None:
                    {
                        var blockInfo = new AssetBundleBlockInfo()
                        {
                            CompressedSize = (uint)fileDataLength,
                            DecompressedSize = (uint)fileDataLength,
                            Flags = 0x00
                        };

                        totalCompressedSize += blockInfo.CompressedSize;

                        newBlocks.Add(blockInfo);

                        if (blockInfoAtEnd)
                            bundleDataStream.CopyToCompat(writer.BaseStream);
                        else
                            newStreams.Add(bundleDataStream);
                        break;
                    }
            }

            newMetadata.BlocksInfo = newBlocks.ToArray();

            byte[] blocksInfo;
            using (var memStream = new MemoryStream())
            {
                var infoWriter = new AssetsFileWriter(memStream);
                newMetadata.Write(Header, infoWriter);
                blocksInfo = memStream.ToArray();
            }

            // Listing is usually lz4 even if the data blocks are lzma
            var compressedBlocksInfo = Lz4Helper.Compress(blocksInfo);

            var totalFileSize = headerSize + compressedBlocksInfo.Length + totalCompressedSize;
            newHeader.Size = totalFileSize;
            newHeader.DecompressedSize = (uint)blocksInfo.Length;
            newHeader.CompressedSize = (uint)compressedBlocksInfo.Length;

            if (!blockInfoAtEnd)
            {
                writer.Write(compressedBlocksInfo);
                foreach (var newStream in newStreams)
                {
                    newStream.Position = 0;
                    newStream.CopyToCompat(writer.BaseStream);
                    newStream.Close();
                }
            }
            else
            {
                writer.Write(compressedBlocksInfo);
            }

            writer.Position = 0;
            newHeader.Write(writer);
            if (newHeader.Version >= 7)
                writer.Align16();

            return true;
        }

        public int FileCount
        {
            get
            {
                if (Header != null)
                    return Metadata.DirectoryCount;

                return 0;
            }
        }

        public bool IsAssetsFile(int index)
        {
            GetFileRange(index, out var offset, out var length);
            return AssetsFile.IsAssetsFile(Reader, offset, length);
        }

        public int GetFileIndex(string name)
        {
            if (Header != null)
            {
                var dirInf = Metadata.DirectoryInfo;
                for (int i = 0; i < dirInf.Length; i++)
                {
                    var info = dirInf[i];
                    if (info.Name == name)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public string GetFileName(int index)
        {
            if (Header != null)
                return BundleHelper.GetDirInfo(this, index).Name;

            return null;
        }

        internal void GetFileRange(int index, out long offset, out long length)
        {
            // unity 3 version bundles not tested
            if (Header != null)
            {
                var entry = BundleHelper.GetDirInfo(this, index);
                offset = entry.GetAbsolutePos(Header);
                length = entry.DecompressedSize;
            }
            else
            {
                throw new NullReferenceException(nameof(Header));
            }
        }

        private FileStream GetTempFileStream()
        {
            var tempFilePath = Path.GetTempFileName();
            var tempFileStream = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose);
            return tempFileStream;
        }
    }
}
