using System.Collections.Generic;

namespace UnityTools
{
    public static class AssetHelper
    {
        public static AssetClassID FixAudioID(AssetClassID id)
        {
            switch (id)
            {
                case AssetClassID.AudioMixerController:
                    id = AssetClassID.AudioMixer;
                    break;
                case AssetClassID.AudioMixerGroupController:
                    id = AssetClassID.AudioMixerGroup;
                    break;
                case AssetClassID.AudioMixerSnapshotController:
                    id = AssetClassID.AudioMixerSnapshot;
                    break;
            }
            return id;
        }

        public static ClassDatabaseType FindAssetClassByID(ClassDatabaseFile cldb, AssetClassID id)
        {
            id = FixAudioID(id);
            foreach (var type in cldb.classes)
            {
                if (type.classId == id)
                    return type;
            }
            return null;
        }

        public static ClassDatabaseType FindAssetClassByName(ClassDatabaseFile cldb, string name)
        {
            foreach (var type in cldb.classes)
            {
                if (type.name.GetString(cldb) == name)
                    return type;
            }
            return null;
        }

        public static Type_0D FindTypeTreeTypeByID(TypeTree typeTree, AssetClassID id)
        {
            foreach (var type in typeTree.unity5Types)
            {
                if (type.ClassID == id)
                    return type;
            }
            return null;
        }

        public static Type_0D FindTypeTreeTypeByID(TypeTree typeTree, AssetClassID id, ushort scriptIndex)
        {
            foreach (var type in typeTree.unity5Types)
            {
                //5.5+
                if (type.ClassID == id && type.ScriptIndex == scriptIndex)
                    return type;
                //5.4-
                if (type.ClassID < 0 && id is AssetClassID.MonoBehaviour &&
                    (AssetClassID)(type.ScriptIndex - 0x10000) == type.ClassID)
                    return type;
            }
            return null;
        }

        public static Type_0D FindTypeTreeTypeByScriptIndex(TypeTree typeTree, ushort scriptIndex)
        {
            foreach (var type in typeTree.unity5Types)
            {
                if (type.ScriptIndex == scriptIndex)
                    return type;
            }
            return null;
        }

        public static Type_0D FindTypeTreeTypeByName(TypeTree typeTree, string name)
        {
            foreach (var type in typeTree.unity5Types)
            {
                if (type.Children[0].GetTypeString(type.stringTable) == name)
                    return type;
            }
            return null;
        }

        public static ushort GetScriptIndex(AssetsFile file, AssetFileInfoEx info)
        {
            return file.header.Version < 0x10 ?
                info.scriptIndex :
                file.typeTree.unity5Types[info.curFileTypeOrIndex].ScriptIndex;
        }

        public static string GetAssetNameFast(AssetsFile file, ClassDatabaseFile cldb, AssetFileInfoEx info)
        {
            var type = FindAssetClassByID(cldb, info.curFileType);
            var reader = file.reader;

            if (file.typeTree.hasTypeTree)
            {
                var scriptId = GetScriptIndex(file, info);
                var ttType = FindTypeTreeTypeByID(file.typeTree, info.curFileType, scriptId);

                var ttTypeName = ttType.Children[0].GetTypeString(ttType.stringTable);
                if (ttType.Children.Length == 0) return type.name.GetString(cldb); //fallback to cldb
                if (ttType.Children.Length > 1 && ttType.Children[1].GetNameString(ttType.stringTable) == "m_Name")
                {
                    reader.Position = info.absoluteFilePos;
                    return reader.ReadCountStringInt32();
                }
                //todo, use the typetree since we have it already, there could be extra fields
                switch (ttTypeName)
                {
                    case "GameObject":
                    {
                        reader.Position = info.absoluteFilePos;
                        var size = reader.ReadInt32();
                        var componentSize = file.header.Version > 0x10 ? 0x0c : 0x10;
                        reader.Position += size * componentSize;
                        reader.Position += 0x04;
                        return reader.ReadCountStringInt32();
                    }
                    case "MonoBehaviour":
                    {
                        reader.Position = info.absoluteFilePos;
                        reader.Position += 0x1c;
                        var name = reader.ReadCountStringInt32();
                        if (name != "")
                        {
                            return name;
                        }
                        break;
                    }
                }
                return ttTypeName;
            }

            var typeName = type.name.GetString(cldb);
            if (type.fields.Count == 0) return type.name.GetString(cldb);
            if (type.fields.Count > 1 && type.fields[1].fieldName.GetString(cldb) == "m_Name")
            {
                reader.Position = info.absoluteFilePos;
                return reader.ReadCountStringInt32();
            }
            switch (typeName)
            {
                case "GameObject":
                {
                    reader.Position = info.absoluteFilePos;
                    var size = reader.ReadInt32();
                    var componentSize = file.header.Version > 0x10 ? 0x0c : 0x10;
                    reader.Position += size * componentSize;
                    reader.Position += 0x04;
                    return reader.ReadCountStringInt32();
                }
                case "MonoBehaviour":
                {
                    reader.Position = info.absoluteFilePos;
                    reader.Position += 0x1c;
                    var name = reader.ReadCountStringInt32();
                    if (name != "")
                    {
                        return name;
                    }
                    break;
                }
            }
            return typeName;
        }

        //no classdatabase but may not work
        public static string GetAssetNameFastNative(AssetsFile file, AssetFileInfoEx info)
        {
            var reader = file.reader;

            if (AssetsFileExtra.HasName(info.curFileType))
            {
                reader.Position = info.absoluteFilePos;
                return reader.ReadCountStringInt32();
            }
            switch (info.curFileType)
            {
                case AssetClassID.GameObject:
                {
                    reader.Position = info.absoluteFilePos;
                    var size = reader.ReadInt32();
                    var componentSize = file.header.Version > 0x10 ? 0xC : 0x10;
                    reader.Position += size * componentSize;
                    reader.Position += 4;
                    return reader.ReadCountStringInt32();
                }
                case AssetClassID.MonoBehaviour:
                {
                    reader.Position = info.absoluteFilePos;
                    reader.Position += 28;
                    var name = reader.ReadCountStringInt32();
                    if (name != "")
                    {
                        return name;
                    }
                    break;
                }
            }
            return string.Empty;
        }

        public static AssetFileInfoEx GetAssetInfo(this AssetsFileTable table, string name, bool caseSensitive = true)
        {
            if (!caseSensitive)
                name = name.ToLower();
            foreach (var info in table.Info)
            {
                var infoName = GetAssetNameFastNative(table.File, info);
                if (!caseSensitive)
                    infoName = infoName.ToLower();
                if (infoName == name)
                {
                    return info;
                }
            }
            return null;
        }

        public static AssetFileInfoEx GetAssetInfo(this AssetsFileTable table, string name, AssetClassID typeId, bool caseSensitive = true)
        {
            if (!caseSensitive)
                name = name.ToLower();
            foreach (var info in table.Info)
            {
                var infoName = GetAssetNameFastNative(table.File, info);
                if (!caseSensitive)
                    infoName = infoName.ToLower();
                if (info.curFileType == typeId && infoName == name)
                {
                    return info;
                }
            }
            return null;
        }

        public static AssetFileInfoEx GetAssetInfo(this AssetsFileTable table, string name, long pathId, bool caseSensitive = true)
        {
            if (!caseSensitive)
                name = name.ToLower();
            foreach (var info in table.Info)
            {
                var infoName = GetAssetNameFastNative(table.File, info);
                if (!caseSensitive)
                    infoName = infoName.ToLower();
                if (info.index == pathId && infoName == name)
                {
                    return info;
                }
            }
            return null;
        }

        public static List<AssetFileInfoEx> GetAssetsOfType(this AssetsFileTable table, AssetClassID typeId)
        {
            var infos = new List<AssetFileInfoEx>();
            foreach (var info in table.Info)
            {
                if (info.curFileType == typeId)
                {
                    infos.Add(info);
                }
            }
            return infos;
        }
    }
}
