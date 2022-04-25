using System.IO;

namespace UnityTools
{
    public class TextureFile
    {
        public string m_Name;
        public int m_ForcedFallbackFormat;
        public bool m_DownscaleFallback;
        public int m_Width;
        public int m_Height;
        public int m_CompleteImageSize;
        public int m_TextureFormat;
        public int m_MipCount;
        public bool m_MipMap;
        public bool m_IsReadable;
        public bool m_ReadAllowed;
        public bool m_StreamingMipmaps;
        public int m_StreamingMipmapsPriority;
        public int m_ImageCount;
        public int m_TextureDimension;
        public GLTextureSettings m_TextureSettings;
        public int m_LightmapFormat;
        public int m_ColorSpace;
        public byte[] pictureData;
        public StreamingInfo m_StreamData;

        public struct GLTextureSettings
        {
            public int m_FilterMode;
            public int m_Aniso;
            public float m_MipBias;
            public int m_WrapMode;
            public int m_WrapU;
            public int m_WrapV;
            public int m_WrapW;
        }

        public struct StreamingInfo
        {
            public ulong offset;
            public uint size;
            public string path;
        }

        public static TextureFile ReadTextureFile(AssetTypeValueField baseField)
        {
            var texture = new TextureFile();
            AssetTypeValueField tempField;

            texture.m_Name = baseField.Get("m_Name").GetValue().AsString();

            if (!(tempField = baseField.Get("m_ForcedFallbackFormat")).IsDummy())
                texture.m_ForcedFallbackFormat = tempField.GetValue().AsInt();

            if (!(tempField = baseField.Get("m_DownscaleFallback")).IsDummy())
                texture.m_DownscaleFallback = tempField.GetValue().AsBool();

            texture.m_Width = baseField.Get("m_Width").GetValue().AsInt();

            texture.m_Height = baseField.Get("m_Height").GetValue().AsInt();

            if (!(tempField = baseField.Get("m_CompleteImageSize")).IsDummy())
                texture.m_CompleteImageSize = tempField.GetValue().AsInt();

            texture.m_TextureFormat = baseField.Get("m_TextureFormat").GetValue().AsInt();

            if (!(tempField = baseField.Get("m_MipCount")).IsDummy())
                texture.m_MipCount = tempField.GetValue().AsInt();

            if (!(tempField = baseField.Get("m_MipMap")).IsDummy())
                texture.m_MipMap = tempField.GetValue().AsBool();

            texture.m_IsReadable = baseField.Get("m_IsReadable").GetValue().AsBool();

            if (!(tempField = baseField.Get("m_ReadAllowed")).IsDummy())
                texture.m_ReadAllowed = tempField.GetValue().AsBool();

            if (!(tempField = baseField.Get("m_StreamingMipmaps")).IsDummy())
                texture.m_StreamingMipmaps = tempField.GetValue().AsBool();

            if (!(tempField = baseField.Get("m_StreamingMipmapsPriority")).IsDummy())
                texture.m_StreamingMipmapsPriority = tempField.GetValue().AsInt();

            texture.m_ImageCount = baseField.Get("m_ImageCount").GetValue().AsInt();

            texture.m_TextureDimension = baseField.Get("m_TextureDimension").GetValue().AsInt();

            var textureSettings = baseField.Get("m_TextureSettings");

            texture.m_TextureSettings.m_FilterMode = textureSettings.Get("m_FilterMode").GetValue().AsInt();

            texture.m_TextureSettings.m_Aniso = textureSettings.Get("m_Aniso").GetValue().AsInt();

            texture.m_TextureSettings.m_MipBias = textureSettings.Get("m_MipBias").GetValue().AsFloat();

            if (!(tempField = textureSettings.Get("m_WrapMode")).IsDummy())
                texture.m_TextureSettings.m_WrapMode = tempField.GetValue().AsInt();

            if (!(tempField = textureSettings.Get("m_WrapU")).IsDummy())
                texture.m_TextureSettings.m_WrapU = tempField.GetValue().AsInt();

            if (!(tempField = textureSettings.Get("m_WrapV")).IsDummy())
                texture.m_TextureSettings.m_WrapV = tempField.GetValue().AsInt();

            if (!(tempField = textureSettings.Get("m_WrapW")).IsDummy())
                texture.m_TextureSettings.m_WrapW = tempField.GetValue().AsInt();

            if (!(tempField = baseField.Get("m_LightmapFormat")).IsDummy())
                texture.m_LightmapFormat = tempField.GetValue().AsInt();

            if (!(tempField = baseField.Get("m_ColorSpace")).IsDummy())
                texture.m_ColorSpace = tempField.GetValue().AsInt();

            var imageData = baseField.Get("image data");
            if (imageData.TemplateField.valueType == EnumValueTypes.ByteArray)
            {
                texture.pictureData = imageData.GetValue().AsByteArray().data;
            }
            else
            {
                var imageDataSize = imageData.GetValue().AsArray().size;
                texture.pictureData = new byte[imageDataSize];
                for (var i = 0; i < imageDataSize; i++)
                {
                    texture.pictureData[i] = (byte)imageData[i].GetValue().AsInt();
                }
            }

            AssetTypeValueField streamData;

            if (!(streamData = baseField.Get("m_StreamData")).IsDummy())
            {
                texture.m_StreamData.offset = streamData.Get("offset").GetValue().AsUInt64();
                texture.m_StreamData.size = streamData.Get("size").GetValue().AsUInt();
                texture.m_StreamData.path = streamData.Get("path").GetValue().AsString();
            }

            return texture;
        }

        //default setting for unitytools
        //usually you have to cd to the assets file
        public byte[] GetTextureData() => GetTextureData(Directory.GetCurrentDirectory());

        //new functions since I didn't like the way unitytools handled it
        public byte[] GetTextureData(AssetsFileInstance inst) => GetTextureData(Path.GetDirectoryName(inst.path));

        public byte[] GetTextureData(AssetsFile file)
        {
            string path = null;
            if (file.readerPar is FileStream fs)
            {
                path = Path.GetDirectoryName(fs.Name);
            }
            return GetTextureData(path);
        }

        public byte[] GetTextureData(string rootPath)
        {
            if (m_StreamData.size != 0 && m_StreamData.path != string.Empty)
            {
                var fixedStreamPath = m_StreamData.path;
                if (!Path.IsPathRooted(fixedStreamPath) && rootPath != null)
                {
                    fixedStreamPath = Path.Combine(rootPath, fixedStreamPath);
                }
                if (File.Exists(fixedStreamPath))
                {
                    Stream stream = File.OpenRead(fixedStreamPath);
                    stream.Position = (long)m_StreamData.offset;
                    pictureData = new byte[m_StreamData.size];
                    stream.Read(pictureData, 0, (int)m_StreamData.size);
                }
                else
                {
                    return null;
                }
            }
            var width = m_Width;
            var height = m_Height;
            var texFmt = (TextureFormat)m_TextureFormat;
            return GetTextureDataFromBytes(pictureData, texFmt, width, height);
        }

        public static byte[] GetTextureDataFromBytes(byte[] data, TextureFormat texFmt, int width, int height)
        {
            return texFmt switch
            {
                TextureFormat.R8 => RGBADecoders.ReadR8(data, width, height),
                TextureFormat.R16 => RGBADecoders.ReadR16(data, width, height),
                TextureFormat.RG16 => RGBADecoders.ReadRG16(data, width, height),
                TextureFormat.RGB24 => RGBADecoders.ReadRGB24(data, width, height),
                TextureFormat.RGB565 => RGBADecoders.ReadRGB565(data, width, height),
                TextureFormat.RGBA32 => RGBADecoders.ReadRGBA32(data, width, height),
                TextureFormat.ARGB32 => RGBADecoders.ReadARGB32(data, width, height),
                TextureFormat.RGBA4444 => RGBADecoders.ReadRGBA4444(data, width, height),
                TextureFormat.ARGB4444 => RGBADecoders.ReadARGB4444(data, width, height),
                TextureFormat.Alpha8 => RGBADecoders.ReadAlpha8(data, width, height),
                TextureFormat.RHalf => RGBADecoders.ReadRHalf(data, width, height),
                TextureFormat.RGHalf => RGBADecoders.ReadRGHalf(data, width, height),
                TextureFormat.RGBAHalf => RGBADecoders.ReadRGBAHalf(data, width, height),
                TextureFormat.DXT1 => DXTDecoders.ReadDXT1(data, width, height),
                TextureFormat.DXT5 => DXTDecoders.ReadDXT5(data, width, height),
                TextureFormat.BC7 => BC7Decoder.ReadBC7(data, width, height),
                TextureFormat.ETC_RGB4 => ETCDecoders.ReadETC(data, width, height),
                TextureFormat.ETC2_RGB4 => ETCDecoders.ReadETC(data, width, height, true),
                _ => null
            };
        }
    }

    public enum FilterMode
    {
        Point,
        Bilinear,
        Trilinear
    }

    public enum WrapMode
    {
        Repeat,
        Clamp,
        Mirror,
        MirrorOnce
    }

    public enum ColorSpace
    {
        Gamma,
        Linear
    }

    public enum TextureFormat
    {
        /// <summary>
        /// Unity 1.5 or earlier (already in 1.2.2 according to documentation)
        /// </summary>
        Alpha8 = 1,
        /// <summary>
        /// Unity 3.0 (already in 1.2.2)
        /// </summary>
        ARGB4444,
        /// <summary>
        /// Unity 1.5 or earlier (already in 1.2.2)
        /// </summary>
        RGB24,
        /// <summary>
        /// Unity 3.2 (not sure about 1.2.2)
        /// </summary>
        RGBA32,
        /// <summary>
        /// Unity 1.5 or earlier (already in 1.2.2)
        /// </summary>
        ARGB32,
        UNUSED06,
        /// <summary>
        /// Unity 3.0 (already in 1.2.2)
        /// </summary>
        RGB565,
        UNUSED08,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        R16,
        /// <summary>
        /// Unity 2.0 (already in 1.2.2)
        /// </summary>
        DXT1,
        /// <summary>
        /// DXT3 in 1.2.2 ?
        /// </summary>
        UNUSED11,
        /// <summary>
        /// Unity 2.0
        /// </summary>
        DXT5,
        /// <summary>
        /// Unity 4.1
        /// </summary>
        RGBA4444,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        BGRA32New,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        RHalf,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        RGHalf,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        RGBAHalf,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        RFloat,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        RGFloat,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        RGBAFloat,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        YUY2,
        /// <summary>
        /// Unity 5.6
        /// </summary>
        RGB9e5Float,
        UNUSED23,
        /// <summary>
        /// Unity 5.5
        /// </summary>
        BC6H,
        /// <summary>
        /// Unity 5.5
        /// </summary>
        BC7,
        /// <summary>
        /// Unity 5.5
        /// </summary>
        BC4,
        /// <summary>
        /// Unity 5.5
        /// </summary>
        BC5,
        /// <summary>
        /// Unity 5.0 //SupportsTextureFormat version codes 0 (original) and 1 (Unity 2017.3)
        /// </summary>
        DXT1Crunched,
        /// <summary>
        /// Unity 5.0 //SupportsTextureFormat version codes 0 (original) and 1 (Unity 2017.3)
        /// </summary>
        DXT5Crunched,
        /// <summary>
        /// Unity 2.6
        /// </summary>
        PVRTC_RGB2,
        /// <summary>
        /// Unity 2.6
        /// </summary>
        PVRTC_RGBA2,
        /// <summary>
        /// Unity 2.6
        /// </summary>
        PVRTC_RGB4,
        /// <summary>
        /// Unity 2.6
        /// </summary>
        PVRTC_RGBA4,
        /// <summary>
        /// Unity 3.0
        /// </summary>
        ETC_RGB4,
        /// <summary>
        /// Unity 3.4, removed in 2018.1
        /// </summary>
        ATC_RGB4,
        /// <summary>
        /// Unity 3.4, removed in 2018.1
        /// </summary>
        ATC_RGBA8,
        /// <summary>
        /// Unity 3.4, removed in Unity 4.5
        /// </summary>
        BGRA32Old,
        UNUSED38, //TexFmt_ATF_RGB_DXT1, added in Unity 3.5, removed in Unity 5.0
        UNUSED39, //TexFmt_ATF_RGBA_JPG, added in Unity 3.5, removed in Unity 5.0
        UNUSED40, //TexFmt_ATF_RGB_JPG, added in Unity 3.5, removed in Unity 5.0
        /// <summary>
        /// Unity 4.5
        /// </summary>
        EAC_R,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        EAC_R_SIGNED,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        EAC_RG,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        EAC_RG_SIGNED,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ETC2_RGB4,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ETC2_RGBA1,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ETC2_RGBA8,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGB_4x4,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGB_5x5,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGB_6x6,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGB_8x8,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGB_10x10,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGB_12x12,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGBA_4x4,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGBA_5x5,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGBA_6x6,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGBA_8x8,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGBA_10x10,
        /// <summary>
        /// Unity 4.5
        /// </summary>
        ASTC_RGBA_12x12,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        ETC_RGB4_3DS,
        /// <summary>
        /// Unity 5.0
        /// </summary>
        ETC_RGBA8_3DS,
        /// <summary>
        /// Unity 2017.1
        /// </summary>
        RG16,
        /// <summary>
        /// Unity 2017.1
        /// </summary>
        R8,
        /// <summary>
        /// Unity 2017.3  //SupportsTextureFormat version code 1
        /// </summary>
        ETC_RGB4Crunched,
        /// <summary>
        /// Unity 2017.3  //SupportsTextureFormat version code 1
        /// </summary>
        ETC2_RGBA8Crunched
    }
}
