using System.Collections.Generic;
using System.Text;

namespace UnityTools
{
    public struct ClassDatabasePackageHeader
    {
        public string magic;
        public byte fileVersion;
        public byte compressionType;
        public uint stringTableOffset, stringTableLenUncompressed, stringTableLenCompressed;
        public uint fileBlockSize;
        public uint fileCount;
        public List<ClassDatabaseFileRef> files;
        public void Read(EndianReader reader)
        {
            reader.BigEndian = false;
            magic = reader.ReadStringLength(4);
            fileVersion = reader.ReadByte();
            compressionType = reader.ReadByte();
            stringTableOffset = reader.ReadUInt32();
            stringTableLenUncompressed = reader.ReadUInt32();
            stringTableLenCompressed = reader.ReadUInt32();
            if (fileVersion == 1) //is this even right?
                fileBlockSize = reader.ReadUInt32();
            else
                fileBlockSize = 0;
            fileCount = reader.ReadUInt32();
            files = new List<ClassDatabaseFileRef>();
            for (var i = 0; i < fileCount; i++)
            {
                files.Add(new ClassDatabaseFileRef()
                {
                    offset = reader.ReadUInt32(),
                    length = reader.ReadUInt32(),
                    name = reader.ReadStringLength(15)
                });
            }
        }
        public void Write(EndianWriter writer)
        {
            writer.BigEndian = false;
            writer.Write(Encoding.ASCII.GetBytes(magic));
            writer.Write(fileVersion);
            writer.Write(compressionType);
            writer.Write(stringTableOffset);
            writer.Write(stringTableLenUncompressed);
            writer.Write(stringTableLenCompressed);
            if (fileVersion == 1)
                writer.Write(fileBlockSize);
            writer.Write(fileCount);
            for (var i = 0; i < fileCount; i++)
            {
                var fileRef = files[i];
                writer.Write(fileRef.offset);
                writer.Write(fileRef.length);

                var fixedFileName = fileRef.name.PadRight(15, '\0').Substring(0, 15);
                writer.Write(Encoding.ASCII.GetBytes(fixedFileName));
            }
        }
    }
    public struct ClassDatabaseFileRef
    {
        public uint offset;
        public uint length;
        public string name;
    }
}
