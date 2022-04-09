namespace UnityTools
{
    public class TypeField_07
    {
        /// <summary>
        /// Name of the data type. This can be the name of any substructure or a static predefined type.
        /// </summary>
        public string Type;
        /// <summary>
        /// Name of the field.
        /// </summary>
        public string Name;
        /// <summary>
        /// Size of the data value in bytes, e.g. 4 for int. -1 means that there is an array somewhere inside its hierarchy
        /// Note: The padding for the alignment is not included in the size.
        /// </summary>
        public uint ByteSize;
        /// <summary>
        /// Index of the field that is unique within a tree.
        /// Normally starts with 0 and is incremented with each additional field.
        /// </summary>
        public int Index;
        /// <summary>
        /// Array flag, set to 1 if type is "Array" or "TypelessData".
        /// </summary>
        public uint IsArray;
        /// <summary>
        /// Field type version, starts with 1 and is incremented after the type information has been significantly updated in a new release.
        /// Equal to serializedVersion in YAML format files
        /// </summary>
        public uint Version;
        /// <summary>
        /// Metaflags of the field, e.g. the AlignBytesFlag (0x4000) means that the field must be aligned by 4 bytes
        /// </summary>
        public int MetaFlag;
        /// <summary>
        /// Count of children in the array
        /// </summary>
        public uint ChildrenCount;
        /// <summary>
        /// Array of children for this field
        /// </summary>
        public TypeField_07[] Children;

        public void Read(bool hasTypeTree, EndianReader reader, uint version, uint typeVersion = 0) // typeVersion not implemented yet
        {
            reader.BigEndian = true;
            Type = reader.ReadNullTerminated();
            Name = reader.ReadNullTerminated();
            ByteSize = reader.ReadUInt32();
            if (version == 2)
                reader.Position += 0x04;
            Index = version != 3 ? reader.ReadInt32() : -1;
            IsArray = reader.ReadUInt32();
            Version = reader.ReadUInt32();
            MetaFlag = version != 3 ? reader.ReadInt32() : -1;
            if (hasTypeTree)
            {
                ChildrenCount = reader.ReadUInt32();
                Children = new TypeField_07[ChildrenCount];
                for (var i = 0; i < ChildrenCount; i++)
                {
                    Children[i] = new TypeField_07();
                    Children[i].Read(true, reader, version, typeVersion);
                }
            }
        }

        public void Write(bool hasTypeTree, EndianWriter writer)
        {
            writer.BigEndian = true;
            writer.WriteNullTerminated(Type);
            writer.WriteNullTerminated(Name);
            writer.Write(ByteSize);
            writer.Write(Index);
            writer.Write(IsArray);
            writer.Write(Version);
            writer.Write(MetaFlag);
            if (hasTypeTree)
            {
                writer.Write(ChildrenCount);
                for (var i = 0; i < ChildrenCount; i++)
                {
                    Children[i].Write(true, writer);
                }
            }
        }
    }
}
