using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                        ImportXmlDump();
                        break;
                    }
                    case DumpType.JSON:
                        ImportJsonDump();
                        break;
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
            var error = "";

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
                        error += $"An error occurred while writing the value \"{valueStr}\" of type \"{type}\".\n";

                    if (align)
                        Writer.Align();
                }
                else
                {
                    alignStack.Push(align);
                }
            }

            if (error != "")
                throw new Exception(error);
        }

        private static bool WriteData(string type, string value)
        {
            try
            {
                var valueType = AssetTypeValueField.GetValueTypeByTypeName(type);
                switch (valueType)
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
                            var valueStrFix = value[(firstQuote + 1)..(lastQuote - firstQuote)];
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
            // todo
        }

        private static void ImportJsonDump()
        {
            // todo
        }
    }
}
