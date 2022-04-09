using System;
using System.IO;
using System.Text;

namespace UnityTools.EndianIO
{
    /// <summary>
    /// Reads primitive data types as binary values with a specific endianness
    /// </summary>
    public class EndianReader : BinaryReader
    {
        public bool BigEndian = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="EndianReader"/> class based on the specified file path and using UTF-8 encoding, and optionally sets the endianness.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public EndianReader(string path, bool bigEndian = false) : base(File.OpenRead(path))
        {
            BigEndian = bigEndian;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianReader"/> class based on the specified stream and using UTF-8 encoding, and optionally sets the endianness.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public EndianReader(Stream stream, bool bigEndian = false) : base(stream)
        {
            BigEndian = bigEndian;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianReader"/> class based on the specified stream and character encoding, and optionally sets the endianness.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public EndianReader(Stream stream, Encoding encoding, bool bigEndian = false) : base(stream, encoding)
        {
            BigEndian = bigEndian;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianReader"/> class based on the specified stream and character encoding, and optionally sets the endianness and leaves the stream open.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public EndianReader(Stream stream, Encoding encoding, bool leaveOpen, bool bigEndian = false) : base(stream, encoding, leaveOpen)
        {
            BigEndian = bigEndian;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianReader"/> class based on the specified byte array and using UTF-8 encoding, and optionally sets the endianness.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public EndianReader(byte[] data, bool bigEndian = false) : base(new MemoryStream(data))
        {
            BigEndian = bigEndian;
        }

        ~EndianReader()
        {
            Dispose(false);
        }

        public void SwapEndianess() => BigEndian = !BigEndian;

        public override bool ReadBoolean()
        {
            return BinaryConverter.ToBoolean(base.ReadByte());
        }

        public override char ReadChar()
        {
            return BinaryConverter.ToChar(base.ReadByte());
        }

        public override short ReadInt16()
        {
            return BinaryConverter.ToInt16(base.ReadBytes(2), 0, BigEndian);
        }

        public override ushort ReadUInt16()
        {
            return BinaryConverter.ToUInt16(base.ReadBytes(2), 0, BigEndian);
        }

        public int ReadInt24()
        {
            return BinaryConverter.ToInt24(base.ReadBytes(3), 0, BigEndian);
        }

        public uint ReadUInt24()
        {
            return BinaryConverter.ToUInt24(base.ReadBytes(3), 0, BigEndian);
        }

        public override int ReadInt32()
        {
            return BinaryConverter.ToInt32(base.ReadBytes(4), 0, BigEndian);
        }

        public override uint ReadUInt32()
        {
            return BinaryConverter.ToUInt32(base.ReadBytes(4), 0, BigEndian);
        }

        public override long ReadInt64()
        {
            return BinaryConverter.ToInt64(base.ReadBytes(8), 0, BigEndian);
        }

        public override ulong ReadUInt64()
        {
            return BinaryConverter.ToUInt64(base.ReadBytes(8), 0, BigEndian);
        }

        public override float ReadSingle()
        {
            return BinaryConverter.ToSingle(base.ReadBytes(4), 0, BigEndian);
        }

        public override double ReadDouble()
        {
            return BinaryConverter.ToDouble(base.ReadBytes(8), 0, BigEndian);
        }

        public override decimal ReadDecimal()
        {
            return BinaryConverter.ToDecimal(base.ReadBytes(16), 0, BigEndian);
        }

        public void Align(int alignment = 4)
        {
            alignment--;
            Position = (Position + alignment) & ~alignment;
        }

        public string ReadStringLength(int length)
        {
            return Encoding.UTF8.GetString(ReadBytes(length));
        }

        public string ReadNullTerminated()
        {
            var output = new StringBuilder();
            char curChar;
            while ((curChar = ReadChar()) != 0x00)
            {
                output.Append(curChar);
            }
            return output.ToString();
        }

        public static string ReadNullTerminatedArray(byte[] bytes, uint pos)
        {
            var output = new StringBuilder();
            char curChar;
            while ((curChar = (char)bytes[pos]) != 0x00)
            {
                output.Append(curChar);
                pos++;
            }
            return output.ToString();
        }

        public string ReadCountString()
        {
            var length = ReadByte();
            return ReadStringLength(length);
        }

        public string ReadCountStringInt16()
        {
            var length = ReadUInt16();
            return ReadStringLength(length);
        }

        public string ReadCountStringInt32()
        {
            var length = ReadInt32();
            return ReadStringLength(length);
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