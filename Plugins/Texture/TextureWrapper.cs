using System;
using UnityTools;

namespace Plugins.Texture
{
    public class TextureWrapper
    {
        public static byte[] DecodePVRTexLib(byte[] data, int width, int height, TextureFormat format)
        {
            var dest = new byte[width * height * 4];
            uint size;
            unsafe
            {
                fixed (byte* dataPtr = data)
                fixed (byte* destPtr = dest)
                {
                    var dataIntPtr = (IntPtr)dataPtr;
                    var destIntPtr = (IntPtr)destPtr;
                    size = PInvoke.DecodeByPVRTexLib(dataIntPtr, destIntPtr, (int)format, (uint)width, (uint)height);
                }
            }

            if (size is 0)
                return null;

            Array.Resize(ref dest, (int)size);
            return dest;
        }

        public static byte[] EncodePVRTexLib(byte[] data, int level, int space, int width, int height, TextureFormat format)
        {
            var dest = new byte[width * height * 16];
            uint size;
            unsafe
            {
                fixed (byte* dataPtr = data)
                fixed (byte* destPtr = dest)
                {
                    var dataIntPtr = (IntPtr)dataPtr;
                    var destIntPtr = (IntPtr)destPtr;
                    size = PInvoke.EncodeByPVRTexLib(dataIntPtr, destIntPtr, (int)format, level, space, (uint)width, (uint)height);
                }
            }

            if (size is 0)
                return null;

            Array.Resize(ref dest, (int)size);
            return dest;
        }

        public static byte[] EncodeISPC(byte[] data, int level, int width, int height, TextureFormat format)
        {
            var dest = new byte[width * height * 4];
            uint size;
            unsafe
            {
                fixed (byte* dataPtr = data)
                fixed (byte* destPtr = dest)
                {
                    var dataIntPtr = (IntPtr)dataPtr;
                    var destIntPtr = (IntPtr)destPtr;
                    size = PInvoke.EncodeByISPC(dataIntPtr, destIntPtr, (int)format, level, (uint)width, (uint)height);
                }
            }

            if (size is 0)
                return null;

            Array.Resize(ref dest, (int)size);
            return dest;
        }

        public static byte[] DecodeCrunch(byte[] data, int width, int height, TextureFormat format)
        {
            var dest = new byte[width * height * 4];
            uint size;
            unsafe
            {
                fixed (byte* dataPtr = data)
                fixed (byte* destPtr = dest)
                {
                    var dataIntPtr = (IntPtr)dataPtr;
                    var destIntPtr = (IntPtr)destPtr;
                    size = PInvoke.DecodeByCrunchUnity(dataIntPtr, (uint)data.Length, destIntPtr, (int)format, (uint)width, (uint)height);
                }
            }

            if (size is 0)
                return null;

            Array.Resize(ref dest, (int)size);
            return dest;
        }

        public static byte[] EncodeCrunch(byte[] data, int level, int space, int width, int height, TextureFormat format)
        {
            var dest = new byte[width * height * 4];
            uint size;
            unsafe
            {
                fixed (byte* dataPtr = data)
                fixed (byte* destPtr = dest)
                {
                    var dataIntPtr = (IntPtr)dataPtr;
                    var destIntPtr = (IntPtr)destPtr;
                    size = PInvoke.EncodeByCrunchUnity(dataIntPtr, destIntPtr, (int)format, level, space, (uint)width, (uint)height);
                }
            }

            if (size is 0)
                return null;

            Array.Resize(ref dest, (int)size);
            return dest;
        }

        public static byte[] DecodeDetex(byte[] data, int width, int height, TextureFormat format)
        {
            var dest = format switch
            {
                TextureFormat.DXT1 => DXTDecoders.ReadDXT1(data, width, height),
                TextureFormat.DXT5 => DXTDecoders.ReadDXT5(data, width, height),
                _ => BC7Decoder.ReadBC7(data, width, height)
            };

            for (var i = 0; i < dest.Length; i += 4)
            {
                (dest[i + 2], dest[i]) = (dest[i], dest[i + 2]);
            }
            return dest;
        }
    }
}
