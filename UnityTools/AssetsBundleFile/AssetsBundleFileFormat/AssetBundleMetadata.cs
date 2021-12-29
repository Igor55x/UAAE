namespace UnityTools
{
    /// <summary>
    /// Metadata about bundle's block or chunk
    /// </summary>
    public class AssetBundleMetadata
    {
        public Hash128 Hash;
        public int BlockCount;
        public AssetBundleBlockInfo[] BlocksInfo;
        public int DirectoryCount;
        public AssetBundleDirectoryInfo[] DirectoryInfo;

        public void Read(AssetBundleHeader header, AssetsFileReader reader)
        {
            Hash = new Hash128(reader);
            if (header.Version >= 6)
            {
                BlockCount = reader.ReadInt32();
                BlocksInfo = new AssetBundleBlockInfo[BlockCount];
                for (var i = 0; i < BlockCount; i++)
                {
                    BlocksInfo[i] = new AssetBundleBlockInfo();
                    BlocksInfo[i].Read(reader);
                }

                if (header.IsBlocksAndDirectoryInfoCombined())
                {
                    DirectoryCount = reader.ReadInt32();
                    DirectoryInfo = new AssetBundleDirectoryInfo[DirectoryCount];
                    for (var i = 0; i < DirectoryCount; i++)
                    {
                        DirectoryInfo[i] = new AssetBundleDirectoryInfo();
                        DirectoryInfo[i].Read(reader, header.Version);
                    }
                }
            }
            else
            {
                DirectoryCount = reader.ReadInt32();
                DirectoryInfo = new AssetBundleDirectoryInfo[DirectoryCount];
                for (var i = 0; i < DirectoryCount; i++)
                {
                    DirectoryInfo[i] = new AssetBundleDirectoryInfo();
                    DirectoryInfo[i].Read(reader, header.Version);
                }
                reader.Align();
            }
        }

        public void Write(AssetBundleHeader header, AssetsFileWriter writer)
        {
            Hash.Write(writer);
            if (header.Version >= 6)
            {
                BlockCount = BlocksInfo.Length;
                writer.Write(BlockCount);
                for (var i = 0; i < BlockCount; i++)
                {
                    BlocksInfo[i].Write(writer);
                }

                if (header.IsBlocksAndDirectoryInfoCombined())
                {
                    DirectoryCount = DirectoryInfo.Length;
                    writer.Write(DirectoryCount);
                    for (var i = 0; i < DirectoryCount; i++)
                    {
                        DirectoryInfo[i].Write(writer, header.Version);
                    }
                }
            }
            else
            {
                DirectoryCount = DirectoryInfo.Length;
                writer.Write(DirectoryCount);
                for (var i = 0; i < DirectoryCount; i++)
                {
                    DirectoryInfo[i].Write(writer, header.Version);
                }
                writer.Align();
            }
        }
    }
}
