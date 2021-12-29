using UnityTools.Compression.LZ4;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SevenZip.Compression.LZMA;
using System;

namespace UnityTools
{
    public class AssetBundleFile
    {
        public AssetBundleHeader Header;
        public AssetBundleMetadata Metadata;

        public AssetsFileReader Reader;

        public void Close() => Reader.Close();

#warning TODO: add full support for unity 3 version bundles
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
                        Flags = 0x04,
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
                        Reader.BaseStream.CopyToCompat(writer.BaseStream, originalInfo.DecompressedSize);

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
                MemoryStream blocksInfoStream;
                var compressedSize = (int)Header.CompressedSize;
                switch (Header.GetCompressionType())
                {
                    case AssetBundleCompressionType.Lzma:
                        using (var ms = new MemoryStream(reader.ReadBytes(compressedSize)))
                        {
                            blocksInfoStream = SevenZipHelper.StreamDecompress(ms);
                        }
                        break;
                    case AssetBundleCompressionType.Lz4:
                    case AssetBundleCompressionType.Lz4HC:
                        var uncompressedBytes = new byte[Header.DecompressedSize];
                        using (var ms = new MemoryStream(reader.ReadBytes(compressedSize)))
                        {
                            var decoder = new Lz4DecoderStream(ms);
                            decoder.Read(uncompressedBytes, 0, (int)Header.DecompressedSize);
                            decoder.Dispose();
                        }
                        blocksInfoStream = new MemoryStream(uncompressedBytes);
                        break;
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
                    switch (info.GetCompressionType())
                    {
                        case 0:
                            reader.BaseStream.CopyToCompat(writer.BaseStream, info.CompressedSize);
                            break;
                        case 1:
                            SevenZipHelper.StreamDecompress(reader.BaseStream, writer.BaseStream, info.CompressedSize, info.DecompressedSize);
                            break;
                        case 2:
                        case 3:
                            using (var tempMs = new MemoryStream())
                            {
                                reader.BaseStream.CopyToCompat(tempMs, info.CompressedSize);
                                tempMs.Position = 0;

                                using var decoder = new Lz4DecoderStream(tempMs);
                                decoder.CopyToCompat(writer.BaseStream, info.DecompressedSize);
                            }
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

            var newHeader = new AssetBundleHeader
            {
                Signature = Header.Signature,
                Version = Header.Version,
                MinUnityVersion = Header.MinUnityVersion,
                UnityVersion = Header.UnityVersion,
                Size = 0,
                CompressedSize = 0,
                DecompressedSize = 0,
                Flags = 0x43
            };

            var newMetadata = new AssetBundleMetadata
            {
                Hash = new Hash128(new byte[16]),
                BlockCount = 0,
                BlocksInfo = null,
                DirectoryCount = Metadata.DirectoryCount,
                DirectoryInfo = Metadata.DirectoryInfo
            };

            var newBlocks = new List<AssetBundleBlockInfo>();

            reader.Position = Header.GetFileDataOffset();
            var fileDataLength = (int)(Header.Size - reader.Position);
            var fileData = reader.ReadBytes(fileDataLength);

            //todo, we just write everything to memory and then write to file
            //we could calculate the blocks we need ahead of time and correctly
            //size the block listing before this so we can write directly to file
            byte[] compressedFileData;
            switch (compType)
            {
                case AssetBundleCompressionType.Lzma:
                {
                    compressedFileData = SevenZipHelper.Compress(fileData);
                    newBlocks.Add(new AssetBundleBlockInfo
                    {
                        CompressedSize = (uint)compressedFileData.Length,
                        DecompressedSize = (uint)fileData.Length,
                        Flags = 0x41
                    });
                    break;
                }
                case AssetBundleCompressionType.Lz4:
                {
                    using var memStreamCom = new MemoryStream();
                    using var binaryWriter = new BinaryWriter(memStreamCom);
                    using (var memStreamUnc = new MemoryStream(fileData))
                    using (var binaryReader = new BinaryReader(memStreamUnc))
                    {
                        //compress into 0x20000 blocks
                        var uncompressedBlock = binaryReader.ReadBytes(131072);
                        while (uncompressedBlock.Length != 0)
                        {
                            var compressedBlock = LZ4Codec.Encode32HC(uncompressedBlock, 0, uncompressedBlock.Length);

                            if (compressedBlock.Length > uncompressedBlock.Length)
                            {
                                newBlocks.Add(new AssetBundleBlockInfo
                                {
                                    CompressedSize = (uint)uncompressedBlock.Length,
                                    DecompressedSize = (uint)uncompressedBlock.Length,
                                    Flags = 0x0
                                });
                                binaryWriter.Write(uncompressedBlock);
                            }
                            else
                            {
                                newBlocks.Add(new AssetBundleBlockInfo
                                {
                                    CompressedSize = (uint)compressedBlock.Length,
                                    DecompressedSize = (uint)uncompressedBlock.Length,
                                    Flags = 0x3
                                });
                                binaryWriter.Write(compressedBlock);
                            }

                            uncompressedBlock = binaryReader.ReadBytes(131072);
                        }
                    }

                    compressedFileData = memStreamCom.ToArray();
                    break;
                }
                case AssetBundleCompressionType.None:
                {
                    compressedFileData = fileData;
                    newBlocks.Add(new AssetBundleBlockInfo
                    {
                        CompressedSize = (uint)fileData.Length,
                        DecompressedSize = (uint)fileData.Length,
                        Flags = 0x00
                    });
                    break;
                }
                default:
                {
                    return false;
                }
            }

            newMetadata.BlocksInfo = newBlocks.ToArray();

            byte[] bundleInfoBytes;
            using (var memStream = new MemoryStream())
            {
                var memWriter = new AssetsFileWriter(memStream);
                newMetadata.Write(Header, memWriter);
                bundleInfoBytes = memStream.ToArray();
            }

            if (bundleInfoBytes.Length == 0)
                return false;

            //listing is usually lz4 even if the data blocks are lzma
            var bundleInfoBytesCom = LZ4Codec.Encode32HC(bundleInfoBytes, 0, bundleInfoBytes.Length);

            byte[] bundleHeaderBytes;
            using (var memStream = new MemoryStream())
            {
                var memWriter = new AssetsFileWriter(memStream);
                newHeader.Write(memWriter);
                bundleHeaderBytes = memStream.ToArray();
            }

            if (bundleHeaderBytes.Length == 0)
                return false;

            var totalFileSize = (uint)(bundleHeaderBytes.Length + bundleInfoBytesCom.Length + compressedFileData.Length);
            newHeader.Size = totalFileSize;
            newHeader.DecompressedSize = (uint)bundleInfoBytes.Length;
            newHeader.CompressedSize = (uint)bundleInfoBytesCom.Length;

            newHeader.Write(writer);
            if (newHeader.Version >= 7)
                writer.Align16();

            writer.Write(bundleInfoBytesCom);
            writer.Write(compressedFileData);

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
    }
}
