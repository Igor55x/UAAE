using System;
using System.IO;
using System.Text;

namespace UnityTools.EndianIO
{
    /// <summary>
    /// Writes primitive types in binary to a stream with a specific endianness
    /// </summary>
    public class EndianWriter : BinaryWriter
    {
        public bool BigEndian = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="EndianWriter"/> class based on the specified file path and using UTF-8 encoding, and optionally sets the endianness.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public EndianWriter(string path, bool bigEndian = false) : base(File.Open(path, FileMode.Create, FileAccess.Write))
        {
            BigEndian = bigEndian;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianWriter"/> class based on the specified stream and using UTF-8 encoding, and optionally sets the endianness.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public EndianWriter(Stream stream, bool bigEndian = false) : base(stream)
        {
            BigEndian = bigEndian;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianWriter"/> class based on the specified stream and character encoding, and optionally sets the endianness.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public EndianWriter(Stream stream, Encoding encoding, bool bigEndian = false) : base(stream, encoding)
        {
            BigEndian = bigEndian;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianWriter"/> class based on the specified stream and character encoding, and optionally sets the endianness and leaves the stream open.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public EndianWriter(Stream stream, Encoding encoding, bool leaveOpen, bool bigEndian = false) : base(stream, encoding, leaveOpen)
        {
            BigEndian = bigEndian;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianWriter"/> class based on the specified byte array and using UTF-8 encoding, and optionally sets the endianness.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public EndianWriter(byte[] data, bool bigEndian = false) : base(new MemoryStream(data))
        {
            BigEndian = bigEndian;
        }

        ~EndianWriter()
        {
            Dispose(false);
        }

        public void SwapEndianess() => BigEndian = !BigEndian;

        public override void Write(bool value)
        {
            base.Write(value);
        }

        public override void Write(char value)
        {
            base.Write(value);
        }

        public override void Write(short value)
        {
            base.Write(BinaryConverter.GetBytes(value, BigEndian));
        }

        public override void Write(ushort value)
        {
            base.Write(BinaryConverter.GetBytes(value, BigEndian));
        }

        public void WriteInt24(int value)
        {
            base.Write(BinaryConverter.GetBytes(value, true, BigEndian));
        }

        public void WriteUInt24(uint value)
        {
            base.Write(BinaryConverter.GetBytes(value, true, BigEndian));
        }

        public override void Write(int value)
        {
            base.Write(BinaryConverter.GetBytes(value, false, BigEndian));
        }

        public override void Write(uint value)
        {
            base.Write(BinaryConverter.GetBytes(value, false, BigEndian));
        }

        public override void Write(long value)
        {
            base.Write(BinaryConverter.GetBytes(value, BigEndian));
        }

        public override void Write(ulong value)
        {
            base.Write(BinaryConverter.GetBytes(value, BigEndian));
        }

        public override void Write(float value)
        {
            base.Write(BinaryConverter.GetBytes(value, BigEndian));
        }

        public override void Write(double value)
        {
            base.Write(BinaryConverter.GetBytes(value, BigEndian));
        }

        public override void Write(decimal value)
        {
            base.Write(BinaryConverter.GetBytes(value, BigEndian));
        }

        public override void Write(string value)
        {
            base.Write(Encoding.UTF8.GetBytes(value));
        }

        public void Align(int alignment = 4)
        {
            while (Position % alignment != 0) Write((byte)0x00);
        }

        public void WriteNullTerminated(string text)
        {
            Write(text);
            Write((byte)0x00);
        }

        public void WriteCountString(string text)
        {
            if (Encoding.UTF8.GetByteCount(text) > 0xFF)
                throw new Exception("String is longer than 255! Use the Int32 variant instead!");
            Write((byte)Encoding.UTF8.GetByteCount(text));
            Write(text);
        }

        public void WriteCountStringInt16(string text)
        {
            if (Encoding.UTF8.GetByteCount(text) > 0xFFFF)
                throw new Exception("String is longer than 65535! Use the Int32 variant instead!");
            Write((ushort)Encoding.UTF8.GetByteCount(text));
            Write(text);
        }

        public void WriteCountStringInt32(string text)
        {
            Write(Encoding.UTF8.GetByteCount(text));
            Write(text);
        }

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public long Length
        {
            get => BaseStream.Length;
        }
    }
}
