using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml;
using AssetsAdvancedEditor.Utils;
using UnityTools;

namespace AssetsAdvancedEditor.Assets
{
    public static class AssetImporter
    {
        private static StreamReader Reader;
        private static AssetsFileWriter Writer;

        public static AssetsReplacer ImportRawAsset(string path, AssetItem item)
        {
            return AssetModifier.CreateAssetReplacer(item, File.ReadAllBytes(path));
        }

        public static AssetsReplacer ImportDump(string path, AssetItem item, DumpType dumpType)
        {
            using var ms = new MemoryStream();
            Writer = new AssetsFileWriter(ms)
            {
                BigEndian = false
            };
            try
            {
                switch (dumpType)
                {
                    case DumpType.TXT:
                        {
                            using var fs = File.OpenRead(path);
                            using var reader = new StreamReader(fs);
                            Reader = reader;
                            ImportTextDump();
                            break;
                        }
                    case DumpType.XML:
                        {
                            using var fs = File.OpenRead(path);
                            using var reader = new StreamReader(fs);
                            Reader = reader;
                            ImportXmlDump();
                            break;
                        }
                    case DumpType.JSON:
                        {
                            using var fs = File.OpenRead(path);
                            using var reader = new StreamReader(fs);
                            Reader = reader;
                            ImportJsonDump();
                            break;
                        }
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtils.ShowErrorDialog("Something went wrong when reading the dump file:\n" + ex);
                return null;
            }
            return AssetModifier.CreateAssetReplacer(item, ms.ToArray());
        }

        private static void ImportTextDump()
        {
            var alignStack = new Stack<bool>();
            var error = new StringBuilder();

            while (true)
            {
                var line = Reader.ReadLine();
                if (line == null) break;

                var thisDepth = 0;
                while (line[thisDepth] == ' ')
                    thisDepth++;

                if (line[thisDepth] == '[') // array index, ignore
                    continue;

                if (thisDepth < alignStack.Count)
                {
                    while (thisDepth < alignStack.Count)
                    {
                        if (alignStack.Pop())
                            Writer.Align();
                    }
                }

                var align = line.Substring(thisDepth, 1) == "1";
                var typeName = thisDepth + 2;
                var eqSign = line.IndexOf('=');
                var valueStr = line[(eqSign + 1)..].Trim();

                if (eqSign != -1)
                {
                    var type = line[typeName..];
                    type = type.Split()[0];

                    if (type.StartsWith("unsigned"))
                    {
                        type = $"unsigned {type.Split()[1]}";
                    }

                    var success = WriteData(type, valueStr);

                    if (!success)
                        error.Append(string.Format("An error occurred while writing the value \"{0}\" of type \"{1}\".\n", valueStr, type));

                    if (align)
                        Writer.Align();
                }
                else
                {
                    alignStack.Push(align);
                }
            }

            if (error.Length != 0)
                throw new Exception(error.ToString());
        }

        private static bool WriteData(string type, string value)
        {
            var evt = AssetTypeValueField.GetValueTypeByTypeName(type);
            return WriteData(evt, value);
        }
        
        private static bool WriteData(EnumValueTypes evt, string value)
        {
            try
            {
                switch (evt)
                {
                    case EnumValueTypes.Bool:
                        Writer.Write(bool.Parse(value));
                        break;
                    case EnumValueTypes.Int8:
                        Writer.Write(sbyte.Parse(value));
                        break;
                    case EnumValueTypes.UInt8:
                        Writer.Write(byte.Parse(value));
                        break;
                    case EnumValueTypes.Int16:
                        Writer.Write(short.Parse(value));
                        break;
                    case EnumValueTypes.UInt16:
                        Writer.Write(ushort.Parse(value));
                        break;
                    case EnumValueTypes.Int32:
                        Writer.Write(int.Parse(value));
                        break;
                    case EnumValueTypes.UInt32:
                        Writer.Write(uint.Parse(value));
                        break;
                    case EnumValueTypes.Int64:
                        Writer.Write(long.Parse(value));
                        break;
                    case EnumValueTypes.UInt64:
                        Writer.Write(ulong.Parse(value));
                        break;
                    case EnumValueTypes.Float:
                        Writer.Write(float.Parse(value));
                        break;
                    case EnumValueTypes.Double:
                        Writer.Write(double.Parse(value));
                        break;
                    case EnumValueTypes.String:
                        {
                            var firstQuote = value.IndexOf('"');
                            var lastQuote = value.LastIndexOf('"');
                            var valueStrFix = value;
                            if (firstQuote != -1 && lastQuote != -1)
                            {
                                valueStrFix = value[(firstQuote + 1)..(lastQuote - firstQuote)];
                            }
                            valueStrFix = UnescapeDumpString(valueStrFix);
                            Writer.WriteCountStringInt32(valueStrFix);
                            break;
                        }
                    case EnumValueTypes.None:
                    case EnumValueTypes.Array:
                    case EnumValueTypes.ByteArray:
                        return false;
                    default:
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string UnescapeDumpString(string str)
        {
            var sb = new StringBuilder(str.Length);
            var escaping = false;
            foreach (var c in str)
            {
                if (!escaping && c == '\\')
                {
                    escaping = true;
                    continue;
                }

                if (escaping)
                {
                    if (c == '\\')
                        sb.Append('\\');
                    else if (c == 'r')
                        sb.Append('\r');
                    else if (c == 'n')
                        sb.Append('\n');
                    else
                        sb.Append(c);

                    escaping = false;
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static void ImportXmlDump()
        {
            var doc = new XmlDocument();
            var xml = Reader.ReadToEnd();
            doc.LoadXml(xml);
            RecurseXmlDump(doc.ChildNodes);
        }

        private static void RecurseXmlDump(XmlNodeList nodes)
        {
            var error = new StringBuilder();

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var nodeAttributes = node.Attributes;
                var align = nodeAttributes["Align"].Value == "True";
                var sizeValue = nodeAttributes["Size"]?.Value;
                var type = nodeAttributes["Type"]?.Value ?? node.Name;
                var isArray = sizeValue != null;
                var evt = AssetTypeValueField.GetValueTypeByTypeName(type);

                if (node.NodeType is XmlNodeType.Element)
                {
                    if (isArray)
                    {
                        if (evt != EnumValueTypes.ByteArray)
                        {
                            var size = int.Parse(sizeValue);
                            Writer.Write(size);
                            if (size > 0)
                            {
                                RecurseXmlDump(node.ChildNodes);
                            }
                        }
                        else
                        {
                            var size = int.Parse(nodeAttributes["Size"].Value);
                            Writer.Write(size);
                            var data = Convert.FromBase64String(node.InnerText);
                            Writer.Write(data);
                        }
                    }
                    else if (1 <= (int)evt && (int)evt <= 12)
                    {
                        var valueStr = node.InnerText;
                        var success = WriteData(evt, valueStr);

                        if (!success)
                            error.Append(string.Format("An error occurred while writing the value \"{0}\" of type \"{1}\".\n", valueStr, type));
                    }
                    else
                    {
                        RecurseXmlDump(node.ChildNodes);
                    }

                    if (align)
                        Writer.Align();
                }
            }

            if (error.Length != 0)
            {
                throw new Exception(error.ToString());
            }
        }

        private static void ImportJsonDump()
        {
            var json = Reader.ReadToEnd();
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var rootProperty = root.EnumerateObject().First();
            RecurseJsonDump(rootProperty);
        }

        private static void RecurseJsonDump(JsonProperty rootProperty)
        {
            var error = new StringBuilder();

            var splitName = rootProperty.Name.Split();
            var align = splitName[0] == "1";
            var type = splitName[1];
            if (type == "unsigned")
            {
                type = $"unsigned {splitName[2]}";
            }
            var propertyValue = rootProperty.Value;
            var evt = AssetTypeValueField.GetValueTypeByTypeName(type);

            if (propertyValue.ValueKind is JsonValueKind.Array)
            {
                if (evt != EnumValueTypes.ByteArray)
                {
                    var size = propertyValue.GetArrayLength();
                    Writer.Write(size);

                    var objEnumerator = propertyValue.EnumerateArray().SelectMany(o => o.EnumerateObject());
                    foreach (var property in objEnumerator)
                    {
                        RecurseJsonDump(property);
                    }

                    if (align)
                        Writer.Align();
                }
                else
                {
                    var count = propertyValue.GetArrayLength();
                    var byteArrayData = new byte[count];
                    var i = 0;
                    foreach (var byteItem in propertyValue.EnumerateArray())
                    {
                        byteArrayData[i++] = byteItem.GetByte();
                    }
                    Writer.Write(byteArrayData.Length);
                    Writer.Write(byteArrayData);
                }
            }
            else if (propertyValue.ValueKind is JsonValueKind.Object)
            {
                foreach (var property in propertyValue.EnumerateObject())
                {
                    RecurseJsonDump(property);
                }

                if (align)
                    Writer.Align();
            }
            else
            {
                var valueStr = propertyValue.ToString();
                var success = WriteData(evt, valueStr);

                if (!success)
                    error.Append(string.Format("An error occurred while writing the value \"{0}\" of type \"{1}\".\n", valueStr, type));

                if (align)
                    Writer.Align();
            }

            if (error.Length != 0)
            {
                throw new Exception(error.ToString());
            }
        }
    }
}
