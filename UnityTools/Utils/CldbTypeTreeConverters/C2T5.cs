using System.Collections.Generic;
using System.Linq;

namespace UnityTools
{
    public static class C2T5
    {
        public static Type_0D Cldb2TypeTree(ClassDatabaseFile classes, string name)
        {
            var type = AssetHelper.FindAssetClassByName(classes, name);
            return Cldb2TypeTree(classes, type);
        }

        public static Type_0D Cldb2TypeTree(ClassDatabaseFile classes, AssetClassID id)
        {
            var type = AssetHelper.FindAssetClassByID(classes, id);
            return Cldb2TypeTree(classes, type);
        }

        public static Type_0D Cldb2TypeTree(ClassDatabaseFile classes, ClassDatabaseType type)
        {
            var type0d = new Type_0D
            {
                ClassID = type.classId,
                ChildrenCount = (uint)type.fields.Count,
                ScriptIndex = 0xFFFF,
                IsStrippedType = false,
                ScriptHash = new Hash128(),
                TypeHash = new Hash128()
            };
            var stringTable = "";
            var strTableList = new Dictionary<string, uint>();
            var defTableList = new Dictionary<string, uint>();

            uint strTablePos = 0;
            uint defTablePos = 0;

            var defaultTable = Type_0D.strTable.Substring(Type_0D.strTable.Length - 1).Split('\0');
            foreach (var entry in defaultTable)
            {
                if (entry != "")
                {
                    defTableList.Add(entry, defTablePos);
                    defTablePos += (uint)entry.Length + 1;
                }
            }

            var field0ds = new List<TypeField_0D>();
            for (var i = 0; i < type.fields.Count; i++)
            {
                var field = type.fields[i];
                var fieldName = field.fieldName.GetString(classes);
                var typeName = field.typeName.GetString(classes);
                uint fieldNamePos;
                uint typeNamePos;

                if (strTableList.ContainsKey(fieldName))
                {
                    fieldNamePos = strTableList[fieldName];
                }
                else if (defTableList.ContainsKey(fieldName))
                {
                    fieldNamePos = defTableList[fieldName] + 0x80000000;
                }
                else
                {
                    fieldNamePos = strTablePos;
                    strTableList.Add(fieldName, strTablePos);
                    strTablePos += (uint)fieldName.Length + 1;
                }

                if (strTableList.ContainsKey(typeName))
                {
                    typeNamePos = strTableList[typeName];
                }
                else if (defTableList.ContainsKey(typeName))
                {
                    typeNamePos = defTableList[typeName] + 0x80000000;
                }
                else
                {
                    typeNamePos = strTablePos;
                    strTableList.Add(typeName, strTablePos);
                    strTablePos += (uint)typeName.Length + 1;
                }

                field0ds.Add(new TypeField_0D
                {
                    Level = field.depth,
                    MetaFlag = field.flags2,
                    Index = i,
                    IsArray = field.isArray == 1,
                    NameStrOffset = fieldNamePos,
                    ByteSize = field.size,
                    TypeStrOffset = typeNamePos,
                    Version = field.version
                });
            }

            var sortedStrTableList = strTableList.OrderBy(n => n.Value).ToList();
            foreach (var entry in sortedStrTableList)
            {
                stringTable += entry.Key + '\0';
            }

            type0d.stringTable = stringTable;
            type0d.stringTableLen = (uint)stringTable.Length;
            type0d.Children = field0ds.ToArray();
            return type0d;
        }
    }
}
