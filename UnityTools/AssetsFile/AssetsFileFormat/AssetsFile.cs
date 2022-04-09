using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityTools.Utils;

namespace UnityTools
{
    /// <summary>
    /// Assets files contain binary serialized objects and optional run-time type information.
    /// They have file name extensions like .asset, .assets, .sharedassets but may also have no extension at all
    /// </summary>
    public class AssetsFile
    {
        public AssetsFileHeader header;
        public TypeTree typeTree;

        public PreloadList preloadTable;
        public AssetsFileDependencyList dependencies;
        //public string unknownString;

        public uint AssetTablePos;
        public uint AssetCount;

        public EndianReader reader;
        public Stream readerPar;

        public AssetsFile(EndianReader reader)
        {
            this.reader = reader;
            readerPar = reader.BaseStream;
            
            header = new AssetsFileHeader();
            header.Read(reader);

            typeTree = new TypeTree();
            typeTree.Read(reader, header.Version);
            
            AssetCount = reader.ReadUInt32();
            reader.Align();
            AssetTablePos = (uint)reader.Position;

            var assetInfoSize = AssetFileInfo.GetSize(header.Version);
            if (0x0F <= header.Version && header.Version <= 0x10)
            {
                //for these two versions, the asset info is not aligned
                //for the last entry, so we have to do some weird stuff
                reader.Position += ((assetInfoSize + 3) >> 2 << 2) * (AssetCount - 1) + assetInfoSize;
            }
            else
            {
                reader.Position += assetInfoSize * AssetCount;
            }
            if (header.Version > 0x0B)
            {
                preloadTable = new PreloadList();
                preloadTable.Read(reader);
            }

            dependencies = new AssetsFileDependencyList();
            dependencies.Read(reader);
        }
        
        public void Close() => readerPar.Dispose();

        public void Write(EndianWriter writer, List<AssetsReplacer> replacers, long filePos, ClassDatabaseFile typeMeta = null)
        {
            if (filePos == -1)
                filePos = writer.Position;
            else
                writer.Position = filePos;

            header.Write(writer);

            foreach (var replacer in replacers)
            {
                var replacerClassId = replacer.GetClassID();
                var replacerScriptIndex = replacer.GetMonoScriptID();
                if (!typeTree.unity5Types.Any(t => t.ClassID == replacerClassId && t.ScriptIndex == replacerScriptIndex))
                {
                    Type_0D type = null;

                    if (typeMeta != null)
                    {
                        var cldbType = AssetHelper.FindAssetClassByID(typeMeta, replacerClassId);
                        if (cldbType != null)
                        {
                            type = C2T5.Cldb2TypeTree(typeMeta, cldbType);
                            type.ScriptIndex = replacerScriptIndex;
                        }
                    }

                    if (type == null)
                    {
                        type = new Type_0D
                        {
                            ClassID = replacerClassId,
                            IsStrippedType = false,
                            ScriptIndex = replacerScriptIndex,
                            TypeHash = new Hash128(),
                            ChildrenCount = 0,
                            stringTableLen = 0,
                            stringTable = ""
                        };
                    }

                    typeTree.unity5Types.Add(type);
                }
            }

            typeTree.Write(writer, header.Version);

            var oldAssetInfosByPathId = new Dictionary<long, AssetFileInfo>();
            var replacersByPathId = replacers.ToDictionary(r => r.GetPathID());
            var newAssetInfos = new List<AssetFileInfo>();

            // Collect unchanged assets (that aren't getting removed)
            reader.Position = AssetTablePos;

            for (var i = 0; i < AssetCount; i++)
            {
                var oldAssetInfo = new AssetFileInfo();
                oldAssetInfo.Read(header.Version, reader);
                oldAssetInfosByPathId.Add(oldAssetInfo.index, oldAssetInfo);

                if (replacersByPathId.ContainsKey(oldAssetInfo.index))
                    continue;

                var newAssetInfo = new AssetFileInfo
                {
                    index = oldAssetInfo.index,
                    curFileTypeOrIndex = oldAssetInfo.curFileTypeOrIndex,
                    inheritedUnityClass = oldAssetInfo.inheritedUnityClass,
                    scriptIndex = oldAssetInfo.scriptIndex,
                    stripped = false
                };
                newAssetInfos.Add(newAssetInfo);
            }

            // Collect modified and new assets
            foreach (AssetsReplacer replacer in replacers.Where(r => r.GetReplacementType() == AssetsReplacementType.AddOrModify))
            {
                var newAssetInfo = new AssetFileInfo
                {
                    index = replacer.GetPathID(),
                    inheritedUnityClass = (ushort)replacer.GetClassID(), //for older unity versions
                    scriptIndex = replacer.GetMonoScriptID(),
                    stripped = false
                };

                if (header.Version < 0x10)
                {
                    newAssetInfo.curFileTypeOrIndex = (int)replacer.GetClassID();
                }
                else
                {
                    if (replacer.GetMonoScriptID() == 0xFFFF)
                        newAssetInfo.curFileTypeOrIndex = typeTree.unity5Types.FindIndex(t => t.ClassID == replacer.GetClassID());
                    else
                        newAssetInfo.curFileTypeOrIndex = typeTree.unity5Types.FindIndex(t => t.ClassID == replacer.GetClassID() && t.ScriptIndex == replacer.GetMonoScriptID());
                }

                newAssetInfos.Add(newAssetInfo);
            }

            newAssetInfos.Sort((i1, i2) => i1.index.CompareTo(i2.index));

            // Write asset infos (will write again later on to update the offsets and sizes)
            writer.Write(newAssetInfos.Count);
            writer.Align();
            var newAssetTablePos = writer.Position;
            foreach (AssetFileInfo newAssetInfo in newAssetInfos)
            {
                newAssetInfo.Write(header.Version, writer);
            }

            preloadTable.Write(writer);
            dependencies.Write(writer);

            // Temporary fix for secondaryTypeCount and friends
            if (header.Version >= 0x14)
            {
                writer.Write(0); //secondaryTypeCount
            }

            var newMetadataSize = (uint)(writer.Position - filePos - 0x13); //0x13 is header - "endianness byte"? (if that's what it even is)
            if (header.Version >= 0x16)
            {
                // Remove larger variation fields as well
                newMetadataSize -= 0x1c;
            }

            // For padding only. if all initial data before assetData is more than 0x1000, this is skipped
            if (writer.Position < 0x1000)
            {
                while (writer.Position < 0x1000)
                {
                    writer.Write((byte)0x00);
                }
            }
            else
            {
                if (writer.Position % 16 == 0)
                    writer.Position += 16;
                else
                    writer.Align(16);
            }

            var newDataOffset = writer.Position;

            // Write all asset data
            for (var i = 0; i < newAssetInfos.Count; i++)
            {
                var newAssetInfo = newAssetInfos[i];
                newAssetInfo.curFileOffset = writer.Position - newDataOffset;

                if (replacersByPathId.TryGetValue(newAssetInfo.index, out AssetsReplacer replacer))
                {
                    replacer.Write(writer);
                }
                else
                {
                    var oldAssetInfo = oldAssetInfosByPathId[newAssetInfo.index];
                    reader.Position = header.DataOffset + oldAssetInfo.curFileOffset;
                    reader.BaseStream.CopyToCompat(writer.BaseStream, (int)oldAssetInfo.curFileSize);
                }

                newAssetInfo.curFileSize = (uint)(writer.Position - (newDataOffset + newAssetInfo.curFileOffset));
                if (i != newAssetInfos.Count - 1)
                    writer.Align(8);
            }

            var newFileSize = writer.Position - filePos;

            var newHeader = new AssetsFileHeader()
            {
                MetadataSize = newMetadataSize,
                FileSize = newFileSize,
                Version = header.Version,
                DataOffset = newDataOffset,
                Endianness = header.Endianness,
                Reserved = header.Reserved,
                unknown = header.unknown,
                FromBundle = header.FromBundle
            };

            writer.Position = filePos;
            newHeader.Write(writer);

            // Write new asset infos again (this time with offsets and sizes filled in)
            writer.Position = newAssetTablePos;
            foreach (var newAssetInfo in newAssetInfos)
            {
                newAssetInfo.Write(header.Version, writer);
            }

            // Set writer position back to end of file
            writer.Position = filePos + newFileSize;
        }

        public static bool IsAssetsFile(string filePath)
        {
            using var reader = new EndianReader(filePath, true);
            return IsAssetsFile(reader, 0, reader.Length);
        }

        public static bool IsAssetsFile(EndianReader reader, long offset, long length)
        {
            //todo - not fully implemented
            if (length < 0x30)
                return false;

            reader.Position = offset;
            var possibleHeader = reader.ReadStringLength(5);
            if (possibleHeader == "Unity" || possibleHeader.StartsWith("MZ") || possibleHeader.StartsWith("FSB5"))
                return false;

            reader.Position = offset;
            var metadataSize = reader.ReadUInt32();
            if (metadataSize < 8)
                return false;

            reader.Position = offset + 0x08;
            var possibleFormat = reader.ReadInt32();
            if (possibleFormat > 99)
                return false;

            reader.Position = offset + 0x14;

            if (possibleFormat >= 0x16)
            {
                reader.Position += 0x1C;
            }

            var possibleVersion = "";
            char curChar;
            while (reader.Position < reader.Length && (curChar = reader.ReadChar()) != 0x00)
            {
                possibleVersion += curChar;
                if (possibleVersion.Length > 0xFF)
                {
                    return false;
                }
            }

            var emptyVersion = Regex.Replace(possibleVersion, "[a-zA-Z0-9\\.]", "");
            var fullVersion = Regex.Replace(possibleVersion, "[^a-zA-Z0-9\\.]", "");
            return emptyVersion == "" && fullVersion.Length > 0;
        }

        ///public bool GetAssetFile(ulong fileInfoOffset, EndianReader reader, AssetFile buf, FileStream readerPar);
        ///public ulong GetAssetFileOffs(ulong fileInfoOffset, EndianReader reader, FileStream readerPar);
        ///public bool GetAssetFileByIndex(ulong fileIndex, AssetFile buf, uint size, EndianReader reader, FileStream readerPar);
        ///public ulong GetAssetFileOffsByIndex(ulong fileIndex, EndianReader reader, FileStream readerPar);
        ///public bool GetAssetFileByName(string name, AssetFile buf, uint size, EndianReader reader, FileStream readerPar);
        ///public ulong GetAssetFileOffsByName(string name, EndianReader reader, FileStream readerPar);
        ///public ulong GetAssetFileInfoOffs(ulong fileIndex, EndianReader reader, FileStream readerPar);
        ///public ulong GetAssetFileInfoOffsByName(string name, EndianReader reader, FileStream readerPar);
        ///public ulong GetFileList(EndianReader reader, FileStream readerPar);
    }
}
