namespace UnityTools
{
    public struct Hash128
    {
        public byte[] Data; //16 bytes
        public Hash128(byte[] data)
        {
            Data = data;
        }

        public Hash128(EndianReader reader)
        {
            Data = reader.ReadBytes(16);
        }

        public void Write(EndianWriter writer)
        {
            writer.Write(Data);
        }
    }
}
