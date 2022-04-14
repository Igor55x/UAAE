using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AssetsAdvancedEditor.Assets;
using UnityTools;
using UnityTools.Compression;

namespace AssetsAdvancedEditor.Utils
{
    public static class Extensions
    {
        private static readonly string[] byteSizeSuffixes = new [] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        public static string GetFormattedByteSize(long size)
        {
            size = Math.Abs(size);
            var log = (int)Math.Log(size, 1024);
            var div = Math.Pow(1024, log);
            var num = size / div;
            return $"{num:f2} {byteSizeSuffixes[log]}";
        }

        public static string ReplaceInvalidFileNameChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static void GetAssetItemFast(int fileId, AssetsFileInstance fileInst, ClassDatabaseFile cldb, AssetFileInfoEx info, out AssetItem item)
        {
            var file = fileInst.file;
            var classId = info.curFileType;
            var cldbType = AssetHelper.FindAssetClassByID(cldb, classId);
            var reader = file.reader;
            string name;
            string type;
            var scriptIndex = AssetHelper.GetScriptIndex(file, info);

            const string container = "";
            var typeId = info.curFileType;
            var pathId = info.index;
            const string modified = "";
            var monoId = ushort.MaxValue;

            if (typeId is AssetClassID.MonoBehaviour)
            {
                monoId = (ushort)(uint.MaxValue - info.curFileTypeOrIndex);
            }

            item = new AssetItem
            {
                Cont = new AssetContainer(fileInst),
                ListName = "Unnamed asset",
                Container = container,
                TypeID = typeId,
                FileID = fileId,
                PathID = pathId,
                Size = info.curFileSize,
                Modified = modified,
                Position = info.absoluteFilePos,
                MonoID = monoId
            };

            if (file.typeTree.hasTypeTree)
            {
                var ttType = classId is AssetClassID.MonoBehaviour ?
                    AssetHelper.FindTypeTreeTypeByScriptIndex(file.typeTree, scriptIndex) :
                    AssetHelper.FindTypeTreeTypeByID(file.typeTree, classId);

                type = ttType.Children[0].GetTypeString(ttType.stringTable);
                item.Type = type;
                switch (ttType.Children.Length)
                {
                    case > 1 when ttType.Children[1].GetNameString(ttType.stringTable) == "m_Name":
                    {
                        reader.Position = item.Position;
                        name = reader.ReadCountStringInt32();
                        item.Name = name;
                        if (name != "")
                        {
                            item.ListName = name;
                            return;
                        }
                        break;
                    }
                    default:
                    {
                        switch (type)
                        {
                            case "GameObject":
                            {
                                reader.Position = item.Position;
                                var size = reader.ReadInt32();
                                var componentSize = file.header.Version > 0x10 ? 0x0c : 0x10;
                                reader.Position += size * componentSize;
                                reader.Position += 0x04;
                                name = reader.ReadCountStringInt32();
                                item.Name = name;
                                if (name != "")
                                {
                                    item.ListName = $"{type} {name}";
                                }
                                return;
                            }
                            case "MonoBehaviour":
                            {
                                reader.Position = item.Position;
                                reader.Position += 0x1c;
                                name = reader.ReadCountStringInt32();
                                item.Name = name;
                                if (name != "")
                                {
                                    item.ListName = $"{type} {name}";
                                }
                                return;
                            }
                        }
                        break;
                    }
                }
                return;
            }

            if (cldbType == null)
            {
                item.Type = $"0x{classId:X8}";
                return;
            }

            type = cldbType.name.GetString(cldb);
            item.Type = type;
            switch (cldbType.fields.Count)
            {
                case 0:
                {
                    return;
                }
                case > 1 when cldbType.fields[1].fieldName.GetString(cldb) == "m_Name":
                {
                    reader.Position = item.Position;
                    name = reader.ReadCountStringInt32();
                    item.Name = name;
                    if (name != "")
                    {
                        item.ListName = name;
                    }
                    break;
                }
                default:
                {
                    switch (type)
                    {
                        case "GameObject":
                        {
                            reader.Position = item.Position;
                            var size = reader.ReadInt32();
                            var componentSize = file.header.Version > 0x10 ? 0x0c : 0x10;
                            reader.Position += size * componentSize;
                            reader.Position += 0x04;
                            name = reader.ReadCountStringInt32();
                            item.Name = name;
                            if (name != "")
                            {
                                item.ListName = $"{type} {name}";
                            }
                            return;
                        }
                        case "MonoBehaviour":
                        {
                            reader.Position = item.Position;
                            reader.Position += 0x1c;
                            name = reader.ReadCountStringInt32();
                            item.Name = name;
                            if (name != "")
                            {
                                item.ListName = $"{type} {name}";
                            }
                            return;
                        }
                    }
                    break;
                }
            }
        }

        public static void GetAssetItemFast(ClassDatabaseFile cldb, AssetContainer cont, AssetsReplacer replacer, out AssetItem item)
        {
            var file = cont.FileInstance.file;
            var classId = replacer.GetClassID();
            var cldbType = AssetHelper.FindAssetClassByID(cldb, classId);
            var reader = cont.FileReader;
            string name;
            string type;

            const string container = "";
            var fileId = replacer.GetFileID();
            var pathId = replacer.GetPathID();
            const string modified = "*";
            var monoId = ushort.MaxValue;

            if (classId is AssetClassID.MonoBehaviour)
            {
                monoId = replacer.GetMonoScriptID();
            }

            item = new AssetItem
            {
                Cont = cont,
                ListName = "Unnamed asset",
                Container = container,
                TypeID = classId,
                FileID = fileId,
                PathID = pathId,
                Size = replacer.GetSize(),
                Modified = modified,
                Position = 0L,
                MonoID = monoId
            };

            if (file.typeTree.hasTypeTree)
            {
                var ttType = classId is AssetClassID.MonoBehaviour ?
                    AssetHelper.FindTypeTreeTypeByScriptIndex(file.typeTree, monoId) :
                    AssetHelper.FindTypeTreeTypeByID(file.typeTree, classId);

                type = ttType.Children[0].GetTypeString(ttType.stringTable);
                item.Type = type;
                switch (ttType.Children.Length)
                {
                    case > 1 when ttType.Children[1].GetNameString(ttType.stringTable) == "m_Name":
                        {
                            reader.Position = item.Position;
                            name = reader.ReadCountStringInt32();
                            item.Name = name;
                            if (name != "")
                            {
                                item.ListName = name;
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            switch (type)
                            {
                                case "GameObject":
                                    {
                                        reader.Position = item.Position;
                                        var size = reader.ReadInt32();
                                        var componentSize = file.header.Version > 0x10 ? 0x0c : 0x10;
                                        reader.Position += size * componentSize;
                                        reader.Position += 0x04;
                                        name = reader.ReadCountStringInt32();
                                        item.Name = name;
                                        if (name != "")
                                        {
                                            item.ListName = $"{type} {name}";
                                        }
                                        return;
                                    }
                                case "MonoBehaviour":
                                    {
                                        reader.Position = item.Position;
                                        reader.Position += 0x1c;
                                        name = reader.ReadCountStringInt32();
                                        item.Name = name;
                                        if (name != "")
                                        {
                                            item.ListName = $"{type} {name}";
                                        }
                                        return;
                                    }
                            }
                            break;
                        }
                }
                return;
            }

            if (cldbType == null)
            {
                item.Type = $"0x{classId:X8}";
                return;
            }

            type = cldbType.name.GetString(cldb);
            item.Type = type;
            switch (cldbType.fields.Count)
            {
                case 0:
                    {
                        return;
                    }
                case > 1 when cldbType.fields[1].fieldName.GetString(cldb) == "m_Name":
                    {
                        reader.Position = item.Position;
                        name = reader.ReadCountStringInt32();
                        item.Name = name;
                        if (name != "")
                        {
                            item.ListName = name;
                        }
                        break;
                    }
                default:
                    {
                        switch (type)
                        {
                            case "GameObject":
                                {
                                    reader.Position = item.Position;
                                    var size = reader.ReadInt32();
                                    var componentSize = file.header.Version > 0x10 ? 0x0c : 0x10;
                                    reader.Position += size * componentSize;
                                    reader.Position += 0x04;
                                    name = reader.ReadCountStringInt32();
                                    item.Name = name;
                                    if (name != "")
                                    {
                                        item.ListName = $"{type} {name}";
                                    }
                                    return;
                                }
                            case "MonoBehaviour":
                                {
                                    reader.Position = item.Position;
                                    reader.Position += 0x1c;
                                    name = reader.ReadCountStringInt32();
                                    item.Name = name;
                                    if (name != "")
                                    {
                                        item.ListName = $"{type} {name}";
                                    }
                                    return;
                                }
                        }
                        break;
                    }
            }
        }

        public static bool WildcardMatches(string test, string pattern, bool caseSensitive = true)
        {
            RegexOptions options = 0;
            if (!caseSensitive)
                options |= RegexOptions.IgnoreCase;

            return Regex.IsMatch(test, "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$", options);
        }

        public static bool IsBundleDataCompressed(this AssetBundleFile bundle)
        {
            var reader = bundle.Reader;
            reader.Position = bundle.Header.GetBundleInfoOffset();
            var blocksInfoStream = new MemoryStream();
            var compressedSize = (int)bundle.Header.CompressedSize;
            var decompressedSize = (int)bundle.Header.DecompressedSize;
            var compressedBlock = reader.ReadBytes(compressedSize);
            switch (bundle.Header.GetCompressionType())
            {
                case AssetBundleCompressionType.Lzma:
                    {
                        using var tempMs = new MemoryStream(compressedBlock);
                        LzmaHelper.DecompressStream(tempMs, blocksInfoStream, decompressedSize);
                        break;
                    }
                case AssetBundleCompressionType.Lz4:
                case AssetBundleCompressionType.Lz4HC:
                    {
                        var decompressedBlock = Lz4Helper.Decompress(compressedBlock, decompressedSize);
                        blocksInfoStream = new MemoryStream(decompressedBlock);
                        break;
                    }
                default:
                    {
                        blocksInfoStream = null;
                        break;
                    }
            }

            var uncompressedMetadata = bundle.Metadata;
            if (bundle.Header.GetCompressionType() != 0)
            {
                using var memReader = new EndianReader(blocksInfoStream, true)
                {
                    Position = 0
                };
                uncompressedMetadata = new AssetBundleMetadata();
                uncompressedMetadata.Read(bundle.Header, memReader);
            }

            return uncompressedMetadata.BlocksInfo.Any(inf => inf.GetCompressionType() != 0);
        }
    }
}
