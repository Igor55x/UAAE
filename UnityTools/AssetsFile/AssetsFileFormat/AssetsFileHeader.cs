namespace UnityTools
{
    /// <summary>
    /// The assets file header is found at the beginning of an .assets, .asset, sharedassetsn.assets, leveln files. The header is always using big endian byte order.
    /// </summary>
    public class AssetsFileHeader
    {
        /// <summary>
        /// Size of the metadata parts of the file
        /// </summary>
        public uint MetadataSize;
        /// <summary>
        /// Size of the whole file
        /// </summary>
        public long FileSize;
        /// <summary>
        /// File format version. The number is required for backward compatibility and is normally incremented after the file format has been changed in a major update
        /// </summary>
        public uint Version;
        /// <summary>
        /// Offset to the serialized asset data. It starts at the data for the first asset
        /// </summary>
        public long DataOffset;
        /// <summary>
        /// Presumably controls the byte order of the data structure. This field is normally set to <see langword="false"/>, which may indicate a little endian byte order.
        /// </summary>
        public bool Endianness;
        /// <summary>
        /// Contains 3 unknown bytes
        /// </summary>
        public byte[] Reserved;
        /// <summary>
        /// ???
        /// </summary>
        public uint unknown;
        /// <summary>
        /// If it is 0x1B it means that the assets file was inside the bundle file
        /// </summary>
        public uint FromBundle;

        public int GetSizeBytes()
        {
            if (Version < 9)
                return 0x10;

            return Version >= 16 ? 0x1C : 0x14;
        }

        public void Read(EndianReader reader)
        {
            MetadataSize = reader.ReadUInt32();
            FileSize = reader.ReadUInt32();
            Version = reader.ReadUInt32();
            DataOffset = reader.ReadUInt32();

            if (Version >= 0x09)
            {
                Endianness = reader.ReadBoolean();
                Reserved = reader.ReadBytes(3);
            }
            else
            {
                reader.Position = FileSize - MetadataSize;
                Endianness = reader.ReadBoolean();
            }

            reader.Align();
            if (Version >= 0x16)
            {
                MetadataSize = reader.ReadUInt32();
                FileSize = reader.ReadInt64();
                DataOffset = reader.ReadInt64();
            }
            reader.BigEndian = Endianness;
            if (Version < 0x16) return;
            unknown = reader.ReadUInt32(); //seen as 0x00 everywhere
            FromBundle = reader.ReadUInt32(); //seen as 0x1b in bundles and 0x00 everywhere else
        }

        public void Write(EndianWriter writer)
        {
            writer.BigEndian = true;
            if (Version >= 0x16)
            {
                writer.Write(0);
                writer.Write(0);
                writer.Write(Version);
                writer.Write(0);
            }
            else
            {
                writer.Write(MetadataSize);
                writer.Write((uint)FileSize);
                writer.Write(Version);
                writer.Write((uint)DataOffset);
            }

            if (Version >= 0x09)
            {
                writer.Write(Endianness);
                writer.Write(Reserved);
            }
            else
            {
                writer.Position = FileSize - MetadataSize;
            }

            writer.Align();
            if (Version >= 0x16)
            {
                writer.Write(MetadataSize);
                writer.Write(FileSize);
                writer.Write(DataOffset);
            }
            writer.BigEndian = Endianness;
            if (Version >= 0x16)
            {
                writer.Write(unknown);
                writer.Write(FromBundle);
            }
        }
    }
}
