using AssetsAdvancedEditor.Assets;
using System.IO;
using UnityTools;

namespace Plugins.Texture
{
    public static class TextureHelper
    {
        public static AssetTypeInstance GetByteArrayTexture(AssetsWorkspace workspace, AssetItem tex)
        {
            var textureTemp = workspace.GetTemplateField(tex);
            var imageData = textureTemp.SearchChild("image data");
            if (imageData == null)
                return null;
            imageData.valueType = EnumValueTypes.ByteArray;
            var texTypeInst = new AssetTypeInstance(textureTemp, tex.Cont.FileReader, tex.Position);
            return texTypeInst;
        }

        public static byte[] GetRawTextureBytes(TextureFile texFile, AssetsFileInstance inst)
        {
            var rootPath = Path.GetDirectoryName(inst.path);
            var streamInfo = texFile.m_StreamData;
            var fixedStreamPath = streamInfo.path;
            if (streamInfo.size != 0 && !string.IsNullOrEmpty(fixedStreamPath))
            {
                if (!Path.IsPathRooted(fixedStreamPath) && rootPath != null)
                {
                    fixedStreamPath = Path.Combine(rootPath, fixedStreamPath);
                }
                if (File.Exists(fixedStreamPath))
                {
                    var stream = File.OpenRead(fixedStreamPath);
                    stream.Position = (long)streamInfo.offset;
                    texFile.pictureData = new byte[streamInfo.size];
                    stream.Read(texFile.pictureData, 0, (int)streamInfo.size);
                }
                else
                {
                    return null;
                }
            }
            return texFile.pictureData;
        }

        public static bool GetResSTexture(TextureFile texFile, AssetItem item)
        {
            var parentBundle = item.Cont.FileInstance.parentBundle;
            var streamInfo = texFile.m_StreamData;
            var searchPath = streamInfo.path;
            if (!string.IsNullOrEmpty(searchPath) && parentBundle != null)
            {
                // Some versions apparently don't use archive:/
                if (searchPath.StartsWith("archive:/"))
                    searchPath = searchPath[9..];

                searchPath = Path.GetFileName(searchPath);

                var bundle = parentBundle.file;

                var reader = bundle.Reader;
                foreach (var info in bundle.Metadata.DirectoryInfo)
                {
                    if (info.Name != searchPath) continue;
                    reader.Position = info.GetAbsolutePos(bundle.Header) + (long)streamInfo.offset;
                    texFile.pictureData = reader.ReadBytes((int)streamInfo.size);
                    texFile.m_StreamData.offset = 0;
                    texFile.m_StreamData.size = 0;
                    texFile.m_StreamData.path = "";
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}