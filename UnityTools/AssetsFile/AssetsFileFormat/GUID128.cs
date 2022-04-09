namespace UnityTools
{
    public struct GUID128
    {
        public long mostSignificant;
        public long leastSignificant;
        public void Read(EndianReader reader)
        {
            mostSignificant = reader.ReadInt64();
            leastSignificant = reader.ReadInt64();
        }

        public void Write(EndianWriter writer)
        {
            writer.Write(mostSignificant);
            writer.Write(leastSignificant);
        }
    }
}
