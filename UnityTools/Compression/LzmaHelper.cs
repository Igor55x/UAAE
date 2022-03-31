using SharpCompress.Compressors.LZMA;
using System;
using System.IO;

namespace UnityTools.Compression
{
    public static class LzmaHelper
    {
        private const int PropertiesSize = 5;
        private const int UncompressedSize = 8;

        public static byte[] Compress(byte[] inputBytes)
        {
            using var inputStream = new MemoryStream(inputBytes);
            using var outputStream = new MemoryStream();
            CompressStream(inputStream, outputStream);
            return outputStream.ToArray();
        }

        public static void CompressStream(Stream inputStream, Stream outputStream)
        {
            var properties = new LzmaEncoderProperties(false, 1 << 21, 32);
            using var lzmaStream = new LzmaStream(properties, false, outputStream);
            inputStream.Write(lzmaStream.Properties);
            inputStream.CopyTo(lzmaStream);
        }

        public static byte[] Decompress(byte[] compressedBytes)
        {
            using var inputStream = new MemoryStream(compressedBytes);
            using var outputStream = new MemoryStream();
            DecompressStream(inputStream, outputStream);
            return outputStream.ToArray();
        }

        public static void DecompressStream(Stream inputStream, Stream outputStream)
        {
            // Read the decoder properties
            var properties = new byte[PropertiesSize];
            if (inputStream.Read(properties, 0, PropertiesSize) < PropertiesSize)
            {
                throw new Exception("Input lzma is too short!");
            }

            // Read the decompressed file size.
            var fileSize = new byte[UncompressedSize];
            if (inputStream.Read(fileSize, 0, UncompressedSize) < UncompressedSize)
            {
                throw new Exception("Can't read the file size!");
            }
            var decompressedSize = BitConverter.ToInt64(fileSize);

            // Decode
            var compressedSize = inputStream.Length - inputStream.Position;
            using var lzmaStream = new LzmaStream(properties, inputStream, compressedSize, decompressedSize);
            lzmaStream.CopyTo(outputStream, (int)decompressedSize);
            outputStream.Position = 0;
        }

        public static void DecompressStream(Stream inputStream, Stream outputStream, long decompressedSize)
        {
            // Read the decoder properties
            var properties = new byte[PropertiesSize];
            if (inputStream.Read(properties, 0, PropertiesSize) < PropertiesSize)
            {
                throw new Exception("Input lzma is too short!");
            }

            // Decode
            var compressedSize = inputStream.Length - inputStream.Position;
            using var lzmaStream = new LzmaStream(properties, inputStream, compressedSize, decompressedSize);
            lzmaStream.CopyTo(outputStream, (int)decompressedSize);
            outputStream.Position = 0;
        }

        public static void DecompressStream(Stream inputStream, Stream outputStream, long compressedSize, long decompressedSize)
        {
            var basePosition = inputStream.Position;
            // Read the decoder properties
            var properties = new byte[PropertiesSize];
            if (inputStream.Read(properties, 0, PropertiesSize) < PropertiesSize)
            {
                throw new Exception("Input lzma is too short!");
            }

            // Decode
            using var lzmaStream = new LzmaStream(properties, inputStream, compressedSize - PropertiesSize, decompressedSize);
            lzmaStream.CopyTo(outputStream, (int)decompressedSize);
            inputStream.Position = basePosition + compressedSize;
        }
    }
}
