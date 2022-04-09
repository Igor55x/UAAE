using System.IO;
using System.Text.RegularExpressions;

namespace AssetsAdvancedEditor.Assets
{
    public static class AssetsBundleDetector
    {
        public static DetectedFileType DetectFileType(string filePath)
        {
            string possibleHeader;
            int possibleFormat;
            string emptyVersion;

            using (var fs = File.OpenRead(filePath))
            using (var reader = new EndianReader(fs, true))
            {
                if (fs.Length < 0x20)
                    return DetectedFileType.Unknown;

                possibleHeader = reader.ReadStringLength(5);
                reader.Position = 0x08;
                possibleFormat = reader.ReadInt32();

                reader.Position = possibleFormat >= 0x16 ? 0x30 : 0x14;

                var possibleVersion = "";
                char curChar;
                while (reader.Position < reader.Length && (curChar = reader.ReadChar()) != 0x00)
                {
                    possibleVersion += curChar;
                    if (possibleVersion.Length > 0xFF) break;
                }

                emptyVersion = Regex.Replace(possibleVersion, "[a-zA-Z0-9\\.]", "");
            }

            if (possibleHeader.StartsWith("Unity") || possibleHeader.StartsWith("MZ") || possibleHeader.StartsWith("FSB5"))
            {
                return DetectedFileType.BundleFile;
            }
            if (possibleFormat < 0xFF && emptyVersion == "")
            {
                return DetectedFileType.AssetsFile;
            }
            return DetectedFileType.Unknown;
        }
    }

    public enum DetectedFileType
    {
        Unknown,
        AssetsFile,
        BundleFile
    }
}