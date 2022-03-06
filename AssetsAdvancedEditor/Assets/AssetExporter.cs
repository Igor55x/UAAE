﻿using System;
using System.IO;
using System.Xml;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace AssetsAdvancedEditor.Assets
{
    public static class AssetExporter
    {
        private static StreamWriter Writer;
        private static XmlDocument Doc;

        public static void ExportRawAsset(string path, AssetItem item)
        {
            var br = item.Cont.FileReader;
            br.Position = item.Position;
            var data = br.ReadBytes((int)item.Size);
            File.WriteAllBytes(path, data);
        }

        public static void ExportDump(string path, AssetTypeValueField field, DumpType dumpType)
        {
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
                        Doc.Save(path);
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

        private static void RecurseTextDump(AssetTypeValueField field, int depth = 0)
        {
            var template = field.TemplateField;
            var align = template.align ? "1" : "0";
            var typeName = template.type;
            var fieldName = template.name;
            var isArray = template.isArray;
            var value = field.GetValue();
            var valueType = template.valueType;

            //string's field isn't aligned but its array is
            if (valueType is EnumValueTypes.String)
                align = "1";

            //mainly to handle enum fields not having the int type name
            if (valueType != EnumValueTypes.None &&
                valueType != EnumValueTypes.Array &&
                valueType != EnumValueTypes.ByteArray)
            {
                typeName = CorrectTypeName(valueType);
            }

            if (isArray)
            {
                var sizeTemplate = template.children[0];
                var sizeAlign = sizeTemplate.align ? "1" : "0";
                var sizeTypeName = sizeTemplate.type;
                var sizeFieldName = sizeTemplate.name;
                if (valueType != EnumValueTypes.ByteArray)
                {
                    var size = value.AsArray().size;
                    var isOneItem = size == 1;
                    Writer.WriteLine($"{new string(' ', depth)}{align} {typeName} {fieldName} ({size} {(isOneItem ? "item" : "items")})");
                    Writer.WriteLine($"{new string(' ', depth + 1)}{sizeAlign} {sizeTypeName} {sizeFieldName} = {size}");
                    for (var i = 0; i < field.ChildrenCount; i++)
                    {
                        Writer.WriteLine($"{new string(' ', depth + 1)}[{i}]");
                        RecurseTextDump(field.Children[i], depth + 2);
                    }
                }
                else
                {
                    var byteArray = value.AsByteArray();
                    var data = byteArray.data;
                    var size = (int)byteArray.size;
                    var isOneItem = size == 1;

                    Writer.WriteLine($"{new string(' ', depth)}{align} {typeName} {fieldName} ({size} {(isOneItem ? "item" : "items")})");
                    Writer.WriteLine($"{new string(' ', depth + 1)}{sizeAlign} {sizeTypeName} {sizeFieldName} = {size}");
                    for (var i = 0; i < size; i++)
                    {
                        Writer.WriteLine($"{new string(' ', depth + 1)}[{i}]");
                        Writer.WriteLine($"{new string(' ', depth + 2)}0 UInt8 data = {data[i]}");
                    }
                }
            }
            else
            {
                var valueStr = "";
                if (value != null)
                {
                    var evt = value.GetValueType();
                    if (evt == EnumValueTypes.String)
                    {
                        //only replace \ with \\ but not " with \" lol
                        //you just have to find the last "
                        var fixedStr = value.AsString()
                            .Replace("\\", "\\\\")
                            .Replace("\r", "\\r")
                            .Replace("\n", "\\n");
                        valueStr = $" = \"{fixedStr}\"";
                    }
                    else if (1 <= (int)evt && (int)evt <= 12)
                    {
                        valueStr = $" = {value.AsString()}";
                    }
                }
                Writer.WriteLine($"{new string(' ', depth)}{align} {typeName} {fieldName}{valueStr}");

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

        private static XmlNode RecurseXmlDump(AssetTypeValueField field)
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

            var value = field.GetValue();
            var hasValue = value != null;
            var nodeName = hasValue ? value.GetValueType().ToString() : "Object";
            var e = Doc.CreateElement(isArray ? "Array" : nodeName);
            e.SetAttribute("align", align);

            if (!hasValue)
            {
                e.SetAttribute("Type", typeName);
            }
            e.SetAttribute("Name", fieldName);

            if (isArray)
            {
                var sizeTemplate = template.children[0];
                var sizeAlign = sizeTemplate.align ? "True" : "False";
                var sizeType = sizeTemplate.type;
                var sizeName = sizeTemplate.name;
                var size = value.AsArray().size;
                e.SetAttribute("Size", size.ToString());
                e.SetAttribute("Align", sizeAlign);
                e.SetAttribute("Type", sizeType);
                e.SetAttribute("Name", sizeName);
                for (var i = 0; i < field.ChildrenCount; i++)
                {
                    var result = RecurseXmlDump(field.Children[i]);
                    e.AppendChild(result);
                }
            }
            else
            {
                var valueStr = "";
                if (value != null)
                {
                    var evt = value.GetValueType();
                    if (evt is EnumValueTypes.String)
                    {
                        //only replace \ with \\ but not " with \" lol
                        //you just have to find the last "
                        var fixedStr = value.AsString()
                            .Replace("\\", "\\\\")
                            .Replace("\r", "\\r")
                            .Replace("\n", "\\n");
                        valueStr = fixedStr;
                    }
                    else if (1 <= (int)evt && (int)evt <= 12)
                    {
                        valueStr = value.AsString();
                    }
                    var text = Doc.CreateTextNode(valueStr);
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

        private static void RecurseJsonDump()
        {
            // todo
        }
    }
}
