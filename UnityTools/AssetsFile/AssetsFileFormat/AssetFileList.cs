namespace UnityTools
{
    public class AssetFileList
    {
        public uint sizeFiles;
        public AssetFileInfo[] fileInfs;

        public uint GetSizeBytes(uint version)
        {
            return fileInfs == null ? 0 : (uint) AssetFileInfo.GetSize(version) * sizeFiles + 4;
        }

        public void Read(uint version, EndianReader reader)
        {
            sizeFiles = reader.ReadUInt32();
            fileInfs = new AssetFileInfo[sizeFiles];
            for (var i = 0; i < sizeFiles; i++)
            {
                fileInfs[i] = new AssetFileInfo();
                fileInfs[i].Read(version, reader);
            }
        }

        public void Write(uint version, EndianWriter writer)
        {
            writer.Write(sizeFiles);
            for (var i = 0; i < sizeFiles; i++)
            {
                fileInfs[i].Write(version, writer);
            }
        }
    }
}
