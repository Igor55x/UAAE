using System.IO;
using System.Text.RegularExpressions;
using UnityTools;

namespace AssetsAdvancedEditor.Assets
{
    public static class AssetsBundleDetector
    {
        public static DetectedFileType DetectFileType(string filePath)
        {
            string possibleBundleHeader;
            int possibleFormat;
            string emptyVersion;

            using (var fs = File.OpenRead(filePath))
            using (var reader = new AssetsFileReader(fs))
            {
                if (fs.Length < 0x20) return DetectedFileType.Unknown;
                possibleBundleHeader = reader.ReadStringLength(7);
                reader.Position = 0x08;
                possibleFormat = reader.ReadInt32();
                reader.Position = possibleFormat >= 0x16 ? 0x30 : 0x14;

                if (possibleFormat >= 0x16)
                    reader.Position += 0x1c;

                var possibleVersion = "";
                char curChar;
                while (reader.Position < reader.BaseStream.Length && (curChar = (char) reader.ReadByte()) != 0x00)
                {
                    possibleVersion += curChar;
                    if (possibleVersion.Length > 0xFF) break;
                }

                emptyVersion = Regex.Replace(possibleVersion, "[a-zA-Z0-9\\.]", "");
            }

            if (possibleBundleHeader == "UnityFS")
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