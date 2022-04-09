using System.Text;

namespace UnityTools
{
    public class TypeField_0D
    {
        /// <summary>
        /// Field type version, starts with 1 and is incremented after the type information has been significantly updated in a new release.
        /// Equal to serializedVersion in YAML format files
        /// </summary>
        public ushort Version;
        /// <summary>
        /// Depth of current type relative to the root
        /// </summary>
        public byte Level;
        /// <summary>
        /// Array flag, set to <see langword="true"/> if type is "Array" or "TypelessData".
        /// </summary>
        public bool IsArray;
        /// <summary>
        /// Type offset in <see cref="TypeTree.Type_0D.stringTable">
        /// </summary>
        public uint TypeStrOffset;
        public uint NameStrOffset;
        /// <summary>
        /// Size of the data value in bytes, e.g. 4 for int. -1 means that there is an array somewhere inside its hierarchy
        /// Note: The padding for the alignment is not included in the size.
        /// </summary>
        public int ByteSize;
        /// <summary>
        /// Index of the field that is unique within a tree.
        /// Normally starts with 0 and is incremented with each additional field.
        /// </summary>
        public int Index;
        /// <summary>
        /// Metaflags of the field, e.g. the AlignBytesFlag (0x4000) means that the field must be aligned by 4 bytes
        /// </summary>
        public uint MetaFlag;
        /// <summary>
        /// Reference type hash???
        /// </summary>
        public ulong RefTypeHash;
        public void Read(EndianReader reader, uint format)
        {
            Version = reader.ReadUInt16();
            Level = reader.ReadByte();
            IsArray = reader.ReadBoolean();
            TypeStrOffset = reader.ReadUInt32();
            NameStrOffset = reader.ReadUInt32();
            ByteSize = reader.ReadInt32();
            Index = reader.ReadInt32();
            MetaFlag = reader.ReadUInt32();
            if (format >= 0x12)
            {
                RefTypeHash = reader.ReadUInt64();
            }
        }

        public void Write(EndianWriter writer, uint format)
        {
            writer.Write(Version);
            writer.Write(Level);
            writer.Write(IsArray);
            writer.Write(TypeStrOffset);
            writer.Write(NameStrOffset);
            writer.Write(ByteSize);
            writer.Write(Index);
            writer.Write(MetaFlag);
            if (format >= 0x12)
            {
                writer.Write(RefTypeHash);
            }
        }

        public enum TypeFieldArrayType
        {
            Array       = 0x01,
            Ref         = 0x02,
            Registry    = 0x04,
            ArrayOfRefs = 0x08
        }

        public string GetTypeString(string stringTable)
        {
            var str = new StringBuilder();
            var newTypeStringOffset = TypeStrOffset;
            if (newTypeStringOffset >= 0x80000000)
            {
                newTypeStringOffset -= 0x80000000;
                stringTable = Type_0D.strTable;
            }
            var pos = (int)newTypeStringOffset;
            char c;
            while ((c = stringTable[pos]) != 0x00)
            {
                str.Append(c);
                pos++;
            }
            return str.ToString();
        }

        public string GetNameString(string stringTable)
        {
            var str = new StringBuilder();
            var newNameStringOffset = NameStrOffset;
            if (newNameStringOffset >= 0x80000000)
            {
                newNameStringOffset -= 0x80000000;
                stringTable = Type_0D.strTable;
            }
            var pos = (int)newNameStringOffset;
            char c;
            while ((c = stringTable[pos]) != 0x00)
            {
                str.Append(c);
                pos++;
            }
            return str.ToString();
        }
    }
}
