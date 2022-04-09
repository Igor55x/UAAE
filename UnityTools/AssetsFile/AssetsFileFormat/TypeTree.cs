using System.Collections.Generic;

namespace UnityTools
{
    public class TypeTree
    {
        public string unityVersion;
        public uint version;
        public bool hasTypeTree;
        public int fieldCount;

        public List<Type_0D> unity5Types;
        public List<Type_07> unity4Types;

        public uint bigIDEnabled;

        public void Read(EndianReader reader, uint version)
        {
            unityVersion = reader.ReadNullTerminated();
            this.version = reader.ReadUInt32();
            if (version >= 0x0D)
            {
                hasTypeTree = reader.ReadBoolean();
                fieldCount = reader.ReadInt32();
                unity5Types = new List<Type_0D>();
                for (var i = 0; i < fieldCount; i++)
                {
                    var type0d = new Type_0D();
                    type0d.Read(hasTypeTree, reader, version);
                    unity5Types.Add(type0d);
                }
            }
            else
            {
                hasTypeTree = true;
                fieldCount = reader.ReadInt32();
                unity4Types = new List<Type_07>();
                for (var i = 0; i < fieldCount; i++)
                {
                    var type07 = new Type_07();
                    type07.Read(hasTypeTree, reader, version);
                    unity4Types.Add(type07);
                }
            }
            if (version < 0x0E)
            {
                bigIDEnabled = reader.ReadUInt24();
            }
        }

        public void Write(EndianWriter writer, uint version)
        {
            writer.WriteNullTerminated(unityVersion);
            writer.Write(this.version);
            if (version >= 0x0D)
            {
                writer.Write(hasTypeTree);
                fieldCount = unity5Types.Count;
                writer.Write(fieldCount);
                for (var i = 0; i < fieldCount; i++)
                {
                    unity5Types[i].Write(hasTypeTree, writer, version);
                }
            }
            else
            {
                fieldCount = unity4Types.Count;
                writer.Write(fieldCount);
                for (var i = 0; i < fieldCount; i++)
                {
                    unity4Types[i].Write(hasTypeTree, writer);
                }
            }
            if (version < 0x0E)
            {
                writer.WriteUInt24(bigIDEnabled);
            }
        }
    }
}
