using UnityTools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Size = System.Drawing.Size;
using System.Runtime.CompilerServices;

namespace Plugins.Texture
{
    public class TextureManager
    {
        public static byte[] ImportTexture(string file, TextureFormat format, out Size imgSize)
        {
            using var image = Image.Load<Rgba32>(file);
            imgSize = new Size(image.Width, image.Height);
            var decData = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];

            image.Mutate(i => i.Flip(FlipMode.Vertical));
            image.CopyPixelDataTo(decData);
            if (decData == null)
            {
                return null; //rip
            }

            var encData = EncodeTexture(decData, image.Width, image.Height, format);
            return encData;
        }

        public static bool ExportTexture(byte[] encData, string file, int width, int height, TextureFormat format)
        {
            var decData = DecodeTexture(encData, width, height, format);
            if (decData == null)
                return false;

            var image = Image.LoadPixelData<Rgba32>(decData, width, height);
            image.Mutate(i => i.Flip(FlipMode.Vertical));
            if (file.EndsWith(".png"))
            {
                image.SaveAsPng(file);
            }
            else if (file.EndsWith(".tga"))
            {
                image.SaveAsTga(file);
            }
            else
            {
                image.SaveAsJpeg(file);
            }
            return true;
        }

        public static byte[] DecodeTexture(byte[] data, int width, int height, TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.DXT1Crunched:
                case TextureFormat.DXT5Crunched:
                case TextureFormat.ETC_RGB4Crunched:
                case TextureFormat.ETC2_RGBA8Crunched:
                {
                    var uncrunch = TextureWrapper.DecodeCrunch(data, width, height, format);

                    if (uncrunch == null)
                        return null;

                    switch (format)
                    {
                        case TextureFormat.DXT1Crunched:
                            format = TextureFormat.DXT1;
                            break;
                        case TextureFormat.DXT5Crunched:
                            format = TextureFormat.DXT5;
                            break;
                        case TextureFormat.ETC_RGB4Crunched:
                            format = TextureFormat.ETC_RGB4;
                            break;
                        case TextureFormat.ETC2_RGBA8Crunched:
                            format = TextureFormat.ETC2_RGBA8;
                            break;
                    }

                    var dest = format is TextureFormat.DXT1 or TextureFormat.DXT5 ?
                        TextureWrapper.DecodeDetex(uncrunch, width, height, format) :
                        TextureWrapper.DecodePVRTexLib(uncrunch, width, height, format);

                    return dest;
                }
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32New:
                case TextureFormat.RGBA32:
                case TextureFormat.RGB24:
                case TextureFormat.ARGB4444:
                case TextureFormat.RGBA4444:
                case TextureFormat.RGB565:
                case TextureFormat.Alpha8:
                case TextureFormat.R8:
                case TextureFormat.R16:
                case TextureFormat.RG16:
                case TextureFormat.RHalf:
                case TextureFormat.RGHalf:
                case TextureFormat.RGBAHalf:
                case TextureFormat.RFloat:
                case TextureFormat.RGFloat:
                case TextureFormat.RGBAFloat:
                case TextureFormat.YUY2:
                case TextureFormat.EAC_R:
                case TextureFormat.EAC_R_SIGNED:
                case TextureFormat.EAC_RG:
                case TextureFormat.EAC_RG_SIGNED:
                case TextureFormat.ETC_RGB4:
                case TextureFormat.ETC_RGB4_3DS:
                case TextureFormat.ETC_RGBA8_3DS:
                case TextureFormat.ETC2_RGB4:
                case TextureFormat.ETC2_RGBA1:
                case TextureFormat.ETC2_RGBA8:
                case TextureFormat.PVRTC_RGB2:
                case TextureFormat.PVRTC_RGBA2:
                case TextureFormat.PVRTC_RGB4:
                case TextureFormat.PVRTC_RGBA4:
                case TextureFormat.ASTC_RGB_4x4:
                case TextureFormat.ASTC_RGB_5x5:
                case TextureFormat.ASTC_RGB_6x6:
                case TextureFormat.ASTC_RGB_8x8:
                case TextureFormat.ASTC_RGB_10x10:
                case TextureFormat.ASTC_RGB_12x12:
                case TextureFormat.ASTC_RGBA_4x4:
                case TextureFormat.ASTC_RGBA_5x5:
                case TextureFormat.ASTC_RGBA_8x8:
                case TextureFormat.ASTC_RGBA_10x10:
                case TextureFormat.ASTC_RGBA_12x12:
                {
                    var dest = TextureWrapper.DecodePVRTexLib(data, width, height, format);
                    return dest;
                }
                case TextureFormat.DXT1:
                case TextureFormat.DXT5:
                {
                    var dest = TextureWrapper.DecodeDetex(data, width, height, format);
                    return dest;
                }
                case TextureFormat.BC7:
                {
                    var bc7 = BC7Decoder.ReadBC7(data, width, height);
                    for (var i = 0; i < bc7.Length; i += 4)
                    {
                        var temp = bc7[i];
                        bc7[i] = bc7[i + 2];
                        bc7[i + 2] = temp;
                    }
                    return bc7;
                }
                case TextureFormat.RGB9e5Float:
                case TextureFormat.BC6H:
                case TextureFormat.BC4:
                case TextureFormat.BC5:
                    return null; // todo
                default:
                    return null;
            }
        }

        public static byte[] EncodeTexture(byte[] data, int width, int height, TextureFormat format, int quality = 3)
        {
            switch (format)
            {
                case TextureFormat.DXT1Crunched:
                case TextureFormat.DXT5Crunched:
                case TextureFormat.ETC_RGB4Crunched:
                case TextureFormat.ETC2_RGBA8Crunched:
                {
                    var dest = TextureWrapper.EncodeCrunch(data, quality, 1, width, height, format);
                    return dest;
                }
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32New:
                case TextureFormat.RGBA32:
                case TextureFormat.RGB24:
                case TextureFormat.ARGB4444:
                case TextureFormat.RGBA4444:
                case TextureFormat.RGB565:
                case TextureFormat.Alpha8:
                case TextureFormat.R8:
                case TextureFormat.R16:
                case TextureFormat.RG16:
                case TextureFormat.RHalf:
                case TextureFormat.RGHalf:
                case TextureFormat.RGBAHalf:
                case TextureFormat.RFloat:
                case TextureFormat.RGFloat:
                case TextureFormat.RGBAFloat:
                case TextureFormat.YUY2:
                case TextureFormat.EAC_R:
                case TextureFormat.EAC_R_SIGNED:
                case TextureFormat.EAC_RG:
                case TextureFormat.EAC_RG_SIGNED:
                case TextureFormat.ETC_RGB4:
                case TextureFormat.ETC_RGB4_3DS:
                case TextureFormat.ETC_RGBA8_3DS:
                case TextureFormat.ETC2_RGB4:
                case TextureFormat.ETC2_RGBA1:
                case TextureFormat.ETC2_RGBA8:
                case TextureFormat.PVRTC_RGB2:
                case TextureFormat.PVRTC_RGBA2:
                case TextureFormat.PVRTC_RGB4:
                case TextureFormat.PVRTC_RGBA4:
                case TextureFormat.ASTC_RGB_4x4:
                case TextureFormat.ASTC_RGB_5x5:
                case TextureFormat.ASTC_RGB_6x6:
                case TextureFormat.ASTC_RGB_8x8:
                case TextureFormat.ASTC_RGB_10x10:
                case TextureFormat.ASTC_RGB_12x12:
                case TextureFormat.ASTC_RGBA_4x4:
                case TextureFormat.ASTC_RGBA_5x5:
                case TextureFormat.ASTC_RGBA_8x8:
                case TextureFormat.ASTC_RGBA_10x10:
                case TextureFormat.ASTC_RGBA_12x12:
                case TextureFormat.DXT1:
                case TextureFormat.DXT5:
                {
                    var dest = TextureWrapper.EncodePVRTexLib(data, quality, 1, width, height, format);
                    return dest;
                }
                case TextureFormat.BC6H:
                case TextureFormat.BC7:
                {
                    var dest = TextureWrapper.EncodeISPC(data, quality, width, height, format);
                    return dest;
                }
                case TextureFormat.RGB9e5Float:
                case TextureFormat.BC4:
                case TextureFormat.BC5:
                    return null; // todo
                default:
                    return null;
            }
        }
    }
}
