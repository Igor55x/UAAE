using System.Collections.Generic;

namespace UnityTools
{
    public class ClassDatabaseType
    {
        public AssetClassID classId;
        public int baseClass;
        public ClassDatabaseFileString name;
        public ClassDatabaseFileString assemblyFileName;

        public List<ClassDatabaseTypeField> fields;
        public void Read(EndianReader reader, int version, byte flags)
        {
            classId = (AssetClassID)reader.ReadInt32();
            baseClass = reader.ReadInt32();
            name = new ClassDatabaseFileString();
            name.Read(reader);
            if ((flags & 1) != 0)
            {
                assemblyFileName = new ClassDatabaseFileString();
                assemblyFileName.Read(reader);
            }
            var fieldCount = reader.ReadUInt32();
            fields = new List<ClassDatabaseTypeField>();
            for (var i = 0; i < fieldCount; i++)
            {
                var cdtf = new ClassDatabaseTypeField();
                cdtf.Read(reader, version);
                fields.Add(cdtf);
            }
        }
        public void Write(EndianWriter writer, int version, byte flags)
        {
            writer.Write((int)classId);
            writer.Write(baseClass);
            name.Write(writer);
            if ((flags & 1) != 0)
            {
                assemblyFileName.Write(writer);
            }
            writer.Write(fields.Count);
            for (var i = 0; i < fields.Count; i++)
            {
                fields[i].Write(writer, version);
            }
        }
    }
}
