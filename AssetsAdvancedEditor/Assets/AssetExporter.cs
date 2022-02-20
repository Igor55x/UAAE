using System;
using System.IO;
using System.Xml;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace AssetsAdvancedEditor.Assets
{
    public class AssetExporter
    {
        public AssetsWorkspace Workspace;
        public StreamWriter Writer;
        public string Path;
        public XmlDocument Doc;

        public AssetExporter(AssetsWorkspace workspace) => Workspace = workspace;

        public void ExportRawAsset(string path, AssetItem item)
        {
            Path = path;
            var br = item.Cont.FileReader;
            br.Position = item.Position;
            var data = br.ReadBytes((int)item.Size);
            File.WriteAllBytes(path, data);
        }

        public void ExportDump(string path, AssetItem item, DumpType dumpType)
        {
            ExportDump(path, Workspace.GetBaseField(item), dumpType);
        }

        public void ExportDump(string path, AssetTypeValueField field, DumpType dumpType)
        {
            Path = path;
            try
            {
                switch (dumpType)
                {
                    case DumpType.TXT:
                    {
                        using var fs = File.OpenWrite(path);
                        using var writer = new StreamWriter(fs);
                        Writer = writer;
                        RecurseTextDump(field);
                        break;
                    }
                    case DumpType.XML:
                    {
                        Doc = new XmlDocument();
                        var result = RecurseXmlDump(field);
                        Doc.AppendChild(result);
                        Doc.Save(Path);
                        break;
                    }
                    case DumpType.JSON:
                        RecurseJsonDump();
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtils.ShowErrorDialog($"Something went wrong when writing the {dumpType} dump file.\n" + ex);
            }
        }

        private void RecurseTextDump(AssetTypeValueField field, int depth = 0)
        {
            var template = field.TemplateField;
            var align = template.align ? "1" : "0";
            var typeName = template.type;
            var fieldName = template.name;
            var isArray = template.isArray;

            //string's field isn't aligned but its array is
            if (template.valueType == EnumValueTypes.String)
                align = "1";

            //mainly to handle enum fields not having the int type name
            if (template.valueType != EnumValueTypes.None &&
                template.valueType != EnumValueTypes.Array &&
                template.valueType != EnumValueTypes.ByteArray)
            {
                typeName = CorrectTypeName(template.valueType);
            }

            if (isArray)
            {
                var sizeTemplate = template.children[0];
                var sizeAlign = sizeTemplate.align ? "1" : "0";
                var sizeTypeName = sizeTemplate.type;
                var sizeFieldName = sizeTemplate.name;
                var size = field.GetValue().AsArray().size;
                Writer.WriteLine($"{new string(' ', depth)}{align} {typeName} {fieldName} ({size} {(size != 1 ? "items" : "item")})");
                Writer.WriteLine($"{new string(' ', depth + 1)}{sizeAlign} {sizeTypeName} {sizeFieldName} = {size}");
                for (var i = 0; i < field.ChildrenCount; i++)
                {
                    Writer.WriteLine($"{new string(' ', depth + 1)}[{i}]");
                    RecurseTextDump(field.Children[i], depth + 2);
                }
            }
            else
            {
                var value = "";
                if (field.GetValue() != null)
                {
                    var evt = field.GetValue().GetValueType();
                    if (evt == EnumValueTypes.String)
                    {
                        //only replace \ with \\ but not " with \" lol
                        //you just have to find the last "
                        var fixedStr = field.GetValue().AsString()
                            .Replace("\\", "\\\\")
                            .Replace("\r", "\\r")
                            .Replace("\n", "\\n");
                        value = $" = \"{fixedStr}\"";
                    }
                    else if (1 <= (int)evt && (int)evt <= 12)
                    {
                        value = $" = {field.GetValue().AsString()}";
                    }
                }
                Writer.WriteLine($"{new string(' ', depth)}{align} {typeName} {fieldName}{value}");

                for (var i = 0; i < field.ChildrenCount; i++)
                {
                    RecurseTextDump(field.Children[i], depth + 1);
                }
            }
        }

        private static string CorrectTypeName(EnumValueTypes valueType)
        {
            return valueType switch
            {
                EnumValueTypes.Bool => "bool",
                EnumValueTypes.UInt8 => "UInt8",
                EnumValueTypes.Int8 => "SInt8",
                EnumValueTypes.UInt16 => "UInt16",
                EnumValueTypes.Int16 => "SInt16",
                EnumValueTypes.UInt32 => "unsigned int",
                EnumValueTypes.Int32 => "int",
                EnumValueTypes.UInt64 => "UInt64",
                EnumValueTypes.Int64 => "SInt64",
                EnumValueTypes.Float => "float",
                EnumValueTypes.Double => "double",
                EnumValueTypes.String => "string",
                _ => "UnknownBaseType"
            };
        }

        private XmlNode RecurseXmlDump(AssetTypeValueField field)
        {
            var template = field.TemplateField;
            var align = template.align ? "True" : "False";
            var typeName = template.type;
            var fieldName = template.name;
            var isArray = template.isArray;

            //string's field isn't aligned but its array is
            if (template.valueType == EnumValueTypes.String)
                align = "True";

            //mainly to handle enum fields not having the int type name
            if (template.valueType != EnumValueTypes.None &&
                template.valueType != EnumValueTypes.Array &&
                template.valueType != EnumValueTypes.ByteArray)
            {
                typeName = template.valueType.ToString();
            }

            var hasValue = field.GetValue() != null;
            var nodeName = hasValue ? field.GetValue().GetValueType().ToString() : "Object";
            var e = Doc.CreateElement(isArray ? "Array" : nodeName);
            e.SetAttribute("align", align);

            if (!hasValue)
            {
                e.SetAttribute("typeName", typeName);
            }
            e.SetAttribute("fieldName", fieldName);

            if (isArray)
            {
                var sizeTemplate = template.children[0];
                var sizeAlign = sizeTemplate.align ? "True" : "False";
                var sizeTypeName = sizeTemplate.type;
                var sizeFieldName = sizeTemplate.name;
                var size = field.GetValue().AsArray().size;
                e.SetAttribute("size", size.ToString());
                e.SetAttribute("sizeAlign", sizeAlign);
                e.SetAttribute("sizeTypeName", sizeTypeName);
                e.SetAttribute("sizeFieldName", sizeFieldName);
                for (var i = 0; i < field.ChildrenCount; i++)
                {
                    var result = RecurseXmlDump(field.Children[i]);
                    e.AppendChild(result);
                }
            }
            else
            {
                var value = "";
                if (field.GetValue() != null)
                {
                    var evt = field.GetValue().GetValueType();
                    if (evt == EnumValueTypes.String)
                    {
                        //only replace \ with \\ but not " with \" lol
                        //you just have to find the last "
                        var fixedStr = field.GetValue().AsString()
                            .Replace("\\", "\\\\")
                            .Replace("\r", "\\r")
                            .Replace("\n", "\\n");
                        value = fixedStr;
                    }
                    else if (1 <= (int)evt && (int)evt <= 12)
                    {
                        value = field.GetValue().AsString();
                    }
                    var text = Doc.CreateTextNode(value);
                    e.AppendChild(text);
                }

                for (var i = 0; i < field.ChildrenCount; i++)
                {
                    var result = RecurseXmlDump(field.Children[i]);
                    e.AppendChild(result);
                }
            }
            return e;
        }

        private void RecurseJsonDump()
        {
            // todo
        }
    }
}
