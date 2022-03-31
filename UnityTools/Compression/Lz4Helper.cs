using K4os.Compression.LZ4;
using System;

namespace UnityTools.Compression
{
    public static class Lz4Helper
    {
        public static byte[] Decompress(byte[] source, int decompressedSize)
        {
            var output = new byte[decompressedSize];
            LZ4Codec.Decode(source, output);
            return output;
        }

        public static byte[] Compress(byte[] source)
        {
            var output = new byte[MaximumOutputSize(source.Length)];
            var compressedSize = LZ4Codec.Encode(source, output, LZ4Level.L12_MAX);
            if (compressedSize < 0)
            {
                throw new Exception("Couldn't compress data!");
            }
            Array.Resize(ref output, compressedSize);
            return output;
        }

        public static int MaximumOutputSize(int length)
            => LZ4Codec.MaximumOutputSize(length);
    }
}
