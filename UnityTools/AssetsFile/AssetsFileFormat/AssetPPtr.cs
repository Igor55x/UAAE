namespace UnityTools
{
    public class AssetPPtr
    {
        public int fileID;
        public long pathID;

        public void Read(EndianReader reader)
        {
            fileID = reader.ReadInt32();
            reader.Align();
            pathID = reader.ReadInt64();
            reader.Align();
        }

        public void Write(EndianWriter writer)
        {
            writer.Write(fileID);
            writer.Align();
            writer.Write(pathID);
            writer.Align();
        }
    }
}
