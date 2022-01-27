using System.Collections.Generic;
using System.IO;

namespace UnityTools
{
    public class AssetTypeValueField
    {
        public AssetTypeTemplateField TemplateField;

        public int ChildrenCount;
        public List<AssetTypeValueField> Children;
        public AssetTypeValue Value;

        public void Read(AssetTypeValue value, AssetTypeTemplateField template, List<AssetTypeValueField> children)
        {
            TemplateField = template;
            ChildrenCount = children.Count;
            Children = children;
            Value = value;
        }

        public AssetTypeValueField this[string name]
        {
            get
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    var valueField = Children[i];
                    if (valueField.TemplateField.name == name)
                    {
                        return valueField;
                    }
                }
                return AssetTypeInstance.GetDummyAssetTypeField();
            }
        }

        public AssetTypeValueField this[int index] => Children[index];

        public AssetTypeValueField Get(string name) => this[name];
        public AssetTypeValueField Get(int index) => this[index];

        public string GetName() => TemplateField.name;
        public string GetFieldType() => TemplateField.type;
        public AssetTypeValue GetValue() => Value;
        public void SetChildrenList(List<AssetTypeValueField> children)
        {
            Children = children;
            ChildrenCount = children.Count;
        }

        public void AddChildren(AssetTypeValueField children)
        {
            Children.Add(children);
            ChildrenCount++;
        }

        public void AddChildren(AssetTypeValueField[] children)
        {
            Children.AddRange(children);
            ChildrenCount += children.Length;
        }

        public void RemoveChildren(AssetTypeValueField children)
        {
            Children.Remove(children);
            ChildrenCount--;
        }

        public void RemoveChildren(int index)
        {
            Children.RemoveAt(index);
            ChildrenCount--;
        }

        public bool IsDummy() => ChildrenCount == -1;

        ///public ulong GetByteSize(ulong filePos = 0);

        public static EnumValueTypes GetValueTypeByTypeName(string type)
        {
            switch (type.ToLower())
            {
                case "string":
                    return EnumValueTypes.String;
                case "sint8":
                case "sbyte":
                    return EnumValueTypes.Int8;
                case "uint8":
                case "char":
                case "byte":
                    return EnumValueTypes.UInt8;
                case "sint16":
                case "short":
                    return EnumValueTypes.Int16;
                case "uint16":
                case "unsigned short":
                case "ushort":
                    return EnumValueTypes.UInt16;
                case "sint32":
                case "int":
                    return EnumValueTypes.Int32;
                case "type*":
                    return EnumValueTypes.Int32;
                case "uint32":
                case "unsigned int":
                case "uint":
                    return EnumValueTypes.UInt32;
                case "sint64":
                case "long":
                    return EnumValueTypes.Int64;
                case "uint64":
                case "unsigned long":
                case "ulong":
                case "filesize":
                    return EnumValueTypes.UInt64;
                case "single":
                case "float":
                    return EnumValueTypes.Float;
                case "double":
                    return EnumValueTypes.Double;
                case "bool":
                    return EnumValueTypes.Bool;
                default:
                    return EnumValueTypes.None;
            }
        }

        public void Write(AssetsFileWriter writer, int depth = 0)
        {
            if (TemplateField.isArray)
            {
                if (TemplateField.valueType == EnumValueTypes.ByteArray)
                {
                    var byteArray = GetValue().value.asByteArray;

                    byteArray.size = (uint)byteArray.data.Length;
                    writer.Write(byteArray.size);
                    writer.Write(byteArray.data);
                    if (TemplateField.align) writer.Align();
                }
                else
                {
                    var array = GetValue().value.asArray;

                    array.size = ChildrenCount;
                    writer.Write(array.size);
                    for (var i = 0; i < array.size; i++)
                    {
                        Get(i).Write(writer, depth + 1);
                    }
                    if (TemplateField.align) writer.Align();
                }
            }
            else
            {
                if (ChildrenCount == 0)
                {
                    switch (TemplateField.valueType)
                    {
                        case EnumValueTypes.Int8:
                            writer.Write(GetValue().value.asInt8);
                            if (TemplateField.align) writer.Align();
                            break;
                        case EnumValueTypes.UInt8:
                            writer.Write(GetValue().value.asUInt8);
                            if (TemplateField.align) writer.Align();
                            break;
                        case EnumValueTypes.Bool:
                            writer.Write(GetValue().value.asBool);
                            if (TemplateField.align) writer.Align();
                            break;
                        case EnumValueTypes.Int16:
                            writer.Write(GetValue().value.asInt16);
                            if (TemplateField.align) writer.Align();
                            break;
                        case EnumValueTypes.UInt16:
                            writer.Write(GetValue().value.asUInt16);
                            if (TemplateField.align) writer.Align();
                            break;
                        case EnumValueTypes.Int32:
                            writer.Write(GetValue().value.asInt32);
                            break;
                        case EnumValueTypes.UInt32:
                            writer.Write(GetValue().value.asUInt32);
                            break;
                        case EnumValueTypes.Int64:
                            writer.Write(GetValue().value.asInt64);
                            break;
                        case EnumValueTypes.UInt64:
                            writer.Write(GetValue().value.asUInt64);
                            break;
                        case EnumValueTypes.Float:
                            writer.Write(GetValue().value.asFloat);
                            break;
                        case EnumValueTypes.Double:
                            writer.Write(GetValue().value.asDouble);
                            break;
                        case EnumValueTypes.String:
                            var str = GetValue().value.asString;
                            writer.Write(str.Length);
                            writer.Write(str);
                            writer.Align();
                            break;
                    }
                }
                else
                {
                    for (var i = 0; i < Children.Count; i++)
                    {
                        var child = Children[i];
                        child.Write(writer, depth + 1);
                    }
                    if (TemplateField.align) writer.Align();
                }
            }
        }

        public byte[] WriteToByteArray(bool bigEndian = false)
        {
            using var ms = new MemoryStream();
            using var writer = new AssetsFileWriter(ms)
            {
                BigEndian = bigEndian
            };
            Write(writer);
            return ms.ToArray();
        }
    }
}
