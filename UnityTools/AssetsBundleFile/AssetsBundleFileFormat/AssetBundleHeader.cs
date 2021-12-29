using System;

namespace UnityTools
{
    public class AssetBundleHeader
    {
        /// <summary>
        /// Bundle signature such as UnityFS, UnityRaw, UnityWeb or UnityArchive
        /// </summary>
        public AssetBundleType Signature;
        /// <summary>
        /// Bundle generation
        /// </summary>
        public uint Version;
        /// <summary>
        /// Minimum unity version to support .assets files
        /// </summary>
        public string MinUnityVersion;
        /// <summary>
        /// Actual engine version, which is equal to the unity version of the .assets file that is inside it
        /// </summary>
        public string UnityVersion;

        #region For version 6 and up
        /// <summary>
        /// Equal to file size, sometimes equal to uncompressed data size without the header
        /// </summary>
        public long Size;
        ///// <summary>
        ///// Size of the possibly-compressed (LZMA, LZ4) BlocksInfos, equal to decompressed size if not compressed
        ///// </summary>
        public uint CompressedSize;
        /// <summary>
        /// Decompressed (actual) size of the BlocksInfo
        /// </summary>
        public uint DecompressedSize;
        /// <summary>
        /// Header's flags (equal to 0x00 if not UnityFS)
        /// </summary>
        public uint Flags;
        /// <summary>
        /// ???
        /// </summary>
        public byte Unknown1; // bool???
        #endregion

        #region For version 3 and up
        /// <summary>
        /// Header's hash
        /// </summary>
        public Hash128 Hash;
        /// <summary>
        /// Cyclic redundancy check
        /// </summary>
        public uint Crc;
        /// <summary>
        /// Minimum number of bytes to read for streamed bundles, equal to bundle size for normal bundles
        /// </summary>
        public uint MinimumStreamedBytes;
        /// <summary>
        /// Offset to the first .assets file in DirectoryInfo
        /// </summary>
        public uint DataOffset;
        /// <summary>
        /// Equal to 1 if it's a streamed bundle, number of LZMAChunkInfos + mainData assets otherwise
        /// </summary>
        public uint NumberOfScenesToDownloadBeforeStreaming;
        public uint SceneCount;
        public AssetBundleScene[] Scenes;
        /// <summary>
        /// Size of the whole bundle
        /// </summary>
        public uint FileSize;
        /// <summary>
        /// Decompressed BlocksInfo size
        /// </summary>
        public uint BlocksInfoSize;
        /// <summary>
        /// ???
        /// </summary>
        public byte Unknown2; // bool???
        #endregion

        public void Read(AssetsFileReader reader)
        {
            reader.BigEndian = true;
            Net35Polyfill.TryParse(reader.ReadNullTerminated(), out Signature);
            Version = reader.ReadUInt32();
            MinUnityVersion = reader.ReadNullTerminated();
            UnityVersion = reader.ReadNullTerminated();
            if (Version >= 6)
            {
                Size = reader.ReadInt64();
                CompressedSize = reader.ReadUInt32();
                DecompressedSize = reader.ReadUInt32();
                Flags = reader.ReadUInt32();
                if (Signature != AssetBundleType.UnityFS)
                {
                    Unknown1 = reader.ReadByte();
                }
                if (Version >= 7)
                {
                    reader.Align16();
                }
            }
            else if (Version >= 3)
            {
                if (Version >= 4)
                {
                    Hash = new Hash128(reader);
                    Crc = reader.ReadUInt32();
                }
                MinimumStreamedBytes = reader.ReadUInt32();
                DataOffset = reader.ReadUInt32();
                NumberOfScenesToDownloadBeforeStreaming = reader.ReadUInt32();
                SceneCount = reader.ReadUInt32();
                Scenes = new AssetBundleScene[SceneCount];
                for (var i = 0; i < SceneCount; i++)
                {
                    Scenes[i] = new AssetBundleScene();
                    Scenes[i].Read(reader);
                }

                if (Version >= 2)
                {
                    FileSize = reader.ReadUInt32();
                }
                if (Version >= 3)
                {
                    BlocksInfoSize = reader.ReadUInt32();
                }
                Unknown2 = reader.ReadByte();
                //reader.Align();
            }
            else
            {
                throw new NotSupportedException("AssetBundleHeader.Read : Unknown file version!");
            }
        }

        public void Write(AssetsFileWriter writer)
        {
            writer.BigEndian = true;
            writer.WriteNullTerminated(Signature.ToString());
            writer.Write(Version);
            writer.WriteNullTerminated(MinUnityVersion);
            writer.WriteNullTerminated(UnityVersion);
            if (Version >= 6)
            {
                writer.Write(Size);
                writer.Write(CompressedSize);
                writer.Write(DecompressedSize);
                writer.Write(Flags);
                if (Signature != AssetBundleType.UnityFS)
                {
                    writer.Write(Unknown1);
                }
                if (Version >= 7)
                {
                    writer.Align16();
                }
            }
            else if (Version >= 3)
            {
                if (Version >= 4)
                {
                    Hash.Write(writer);
                    writer.Write(Crc);
                }
                writer.Write(MinimumStreamedBytes);
                writer.Write(DataOffset);
                writer.Write(NumberOfScenesToDownloadBeforeStreaming);
                writer.Write(SceneCount);
                for (var i = 0; i < SceneCount; i++)
                {
                    Scenes[i].Write(writer);
                }

                if (Version >= 2)
                {
                    writer.Write(FileSize);
                }
                if (Version >= 3)
                {
                    writer.Write(BlocksInfoSize);
                }
                writer.Write(Unknown2);
                //writer.Align();
            }
            else
            {
                throw new NotSupportedException("AssetBundleHeader.Write : Unknown file version!");
            }
        }

        public long GetBundleInfoOffset()
        {
            if (IsBlocksInfoAtTheEnd())
            {
                if (Size == 0)
                    return -1;
                return Size - CompressedSize;
            }

            long ret = MinUnityVersion.Length + UnityVersion.Length + 0x1A;
            if (Version >= 7)
            {
                if (IsOldWebPluginCompatibility())
                    return ((ret + 0x0A) + 15) >> 4 << 4;

                return ((ret + Signature.ToString().Length + 1) + 15) >> 4 << 4;
            }

            if (IsOldWebPluginCompatibility())
                return ret + 0x0A;

            return ret + Signature.ToString().Length + 1;
        }

        public long GetFileDataOffset()
        {
            long ret = 0;
            switch (Signature)
            {
                case AssetBundleType.UnityArchive:
                    return CompressedSize;
                case AssetBundleType.UnityFS:
                case AssetBundleType.UnityWeb:
                {
                    ret = MinUnityVersion.Length + UnityVersion.Length + 0x1A;
                    if (IsOldWebPluginCompatibility())
                        ret += 0x0A;
                    else
                        ret += Signature.ToString().Length + 1;
                    break;
                }
            }

            if (Version >= 7)
                ret = (ret + 15) >> 4 << 4;

            if (!IsBlocksInfoAtTheEnd())
                ret += CompressedSize;
            return ret;
        }

        public AssetBundleCompressionType GetCompressionType()
        {
            return (AssetBundleCompressionType)(Flags & 0x3F);
        }

        public bool IsBlocksAndDirectoryInfoCombined()
        {
            return (Flags & 0x40) != 0;
        }

        public bool IsBlocksInfoAtTheEnd()
        {
            return (Flags & 0x80) != 0;
        }

        public bool IsOldWebPluginCompatibility()
        {
            return (Flags & 0x100) != 0;
        }
    }
}
