namespace UnityTools
{
    /// <summary>
    /// Contains compression information about block
    /// Block is a similar to chunk structure that contains data blob but without file entries
    /// </summary>
    public class AssetBundleBlockInfo
    {
        public uint DecompressedSize;
        public uint CompressedSize;
        public ushort Flags;

        public void Read(EndianReader reader)
        {
            DecompressedSize = reader.ReadUInt32();
            CompressedSize = reader.ReadUInt32();
            Flags = reader.ReadUInt16();
        }

        public void Write(EndianWriter writer)
        {
            writer.Write(DecompressedSize);
            writer.Write(CompressedSize);
            writer.Write(Flags);
        }

        public byte GetCompressionType()
        {
            return (byte)(Flags & 0x3F);
        }
    }
}
