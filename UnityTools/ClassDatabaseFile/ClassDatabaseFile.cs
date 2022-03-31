using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityTools.Compression;

namespace UnityTools
{
    public class ClassDatabaseFile
    {
        public bool valid;
        public ClassDatabaseFileHeader header;

        public List<ClassDatabaseType> classes;

        public byte[] stringTable;

        public bool Read(AssetsFileReader reader)
        {
            header = new ClassDatabaseFileHeader();
            header.Read(reader);
            if (header.header != "cldb" || header.fileVersion is > 4 or < 1)
            {
                valid = false;
                return valid;
            }
            classes = new List<ClassDatabaseType>();

            var classTablePos = reader.Position;
            var newReader = reader;
            if (header.compressionType != 0)
            {
                classTablePos = 0;
                var compressedBlock = reader.ReadBytes((int)header.compressedSize);
                var uncompressedSize = (int)header.uncompressedSize;
                var ms = new MemoryStream();
                if (header.compressionType == 1) //lz4
                {
                    var decompressedBlock = Lz4Helper.Decompress(compressedBlock, uncompressedSize);
                    ms = new MemoryStream(decompressedBlock);
                }
                else if (header.compressionType == 2) //lzma
                {
                    using var tempMs = new MemoryStream(compressedBlock);
                    LzmaHelper.DecompressStream(tempMs, ms);
                }
                else
                {
                    valid = false;
                    return valid;
                }

                newReader = new AssetsFileReader(ms)
                {
                    BigEndian = false
                };
            }

            newReader.Position = header.stringTablePos;
            stringTable = newReader.ReadBytes((int)header.stringTableLen);
            newReader.Position = classTablePos;
            var size = newReader.ReadUInt32();
            for (var i = 0; i < size; i++)
            {
                var cdt = new ClassDatabaseType();
                cdt.Read(newReader, header.fileVersion, header.flags);
                classes.Add(cdt);
            }
            valid = true;
            return valid;
        }

        public void Write(AssetsFileWriter writer, int optimizeStringTable = 1, int compress = 1, bool writeStringTable = true)
        {
            var filePos = writer.BaseStream.Position;

            var newStrTable = stringTable;

            //"optimize string table (slow)" mode 2 not supported
            //ex: >AABB\0>localAABB\0 can be just >local>AABB\0
            if (optimizeStringTable == 1)
            {
                var strTableBuilder = new StringBuilder();
                var strTableMap = new Dictionary<string, uint>();
                for (var i = 0; i < classes.Count; i++)
                {
                    var type = classes[i];

                    AddStringTableEntry(strTableBuilder, strTableMap, ref type.name);

                    if (header.fileVersion == 4 && (header.flags & 1) != 0)
                    {
                        AddStringTableEntry(strTableBuilder, strTableMap, ref type.assemblyFileName);
                    }

                    var fields = type.fields;
                    for (var j = 0; j < fields.Count; j++)
                    {
                        var field = fields[j];
                        AddStringTableEntry(strTableBuilder, strTableMap, ref field.fieldName);
                        AddStringTableEntry(strTableBuilder, strTableMap, ref field.typeName);
                        fields[j] = field;
                    }
                }
            }

            header.Write(writer);
            writer.Write(classes.Count);
            for (var i = 0; i < classes.Count; i++)
            {
                classes[i].Write(writer, header.fileVersion, header.flags);
            }

            var stringTablePos = writer.Position;

            //set false only for tpk packing, don't set false anytime else!
            if (writeStringTable)
            {
                writer.Write(newStrTable);
            }

            var fileEndPos = writer.Position;

            var stringTableLen = writer.Position - stringTablePos;
            var fileSize = writer.Position;

            header.stringTablePos = (uint)stringTablePos;
            header.stringTableLen = (uint)stringTableLen;
            header.uncompressedSize = (uint)fileSize;

            writer.Position = filePos;
            header.Write(writer);

            writer.Position = fileEndPos;
        }

        private void AddStringTableEntry(StringBuilder strTable, Dictionary<string, uint> strMap, ref ClassDatabaseFileString str)
        {
            var stringValue = str.GetString(this);

            if (!strMap.ContainsKey(stringValue))
            {
                strMap[stringValue] = (uint)strTable.Length;
                strTable.Append(stringValue + '\0');
            }
            str.str.stringTableOffset = strMap[stringValue];
        }

        public bool IsValid() => valid;
    }
}
