using System;

namespace UnityTools.EndianIO
{
    public static class BinaryConverter
    {
        public static bool ToBoolean(byte value)
        {
            return value != 0;
        }

        public static char ToChar(byte value)
        {
            return (char)value;
        }

        public static short ToInt16(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            return (short)(bigEndian ?
                (buffer[offset + 1] << 0) | (buffer[offset] << 8) :
                (buffer[offset] << 0) | (buffer[offset + 1] << 8));
        }

        public static ushort ToUInt16(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            return unchecked((ushort)ToInt16(buffer, offset, bigEndian));
        }

        public static int ToInt24(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            var value = 0;
            if (bigEndian)
            {
                for (var i = 0; i < 3; i++)
                {
                    value |= buffer[offset + i] << (8 * (2 - i));
                }
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    value |= buffer[offset + i] << (8 * i);
                }
            }
            return value;
        }

        public static uint ToUInt24(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            return unchecked((uint)ToInt24(buffer, offset, bigEndian));
        }

        public static int ToInt32(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            var value = 0;
            if (bigEndian)
            {
                for (var i = 0; i < 4; i++)
                {
                    value |= buffer[offset + i] << (8 * (3 - i));
                }
            }
            else
            {
                for (var i = 0; i < 4; i++)
                {
                    value |= buffer[offset + i] << (8 * i);
                }
            }
            return value;
        }

        public static uint ToUInt32(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            return unchecked((uint)ToInt32(buffer, offset, bigEndian));
        }

        public static long ToInt64(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            var value = 0L;
            if (bigEndian)
            {
                for (var i = 0; i < 8; i++)
                {
                    value |= (long)buffer[offset + i] << (8 * (7 - i));
                }
            }
            else
            {
                for (var i = 0; i < 8; i++)
                {
                    value |= (long)buffer[offset + i] << (8 * i);
                }
            }
            return value;
        }

        public static ulong ToUInt64(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            return unchecked((ulong)ToInt64(buffer, offset, bigEndian));
        }

        public static float ToSingle(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            return Convert.ToSingle(ToInt32(buffer, offset, bigEndian));
        }

        public static double ToDouble(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            return Convert.ToDouble(ToInt64(buffer, offset, bigEndian));
        }

        public static decimal ToDecimal(byte[] buffer, int offset = 0, bool bigEndian = false)
        {
            var bits = new int[4];
            if (bigEndian)
            {
                for (var i = 0; i < 16; i++)
                {
                    bits[i / 4] |= buffer[offset + i] << (8 * (15 - i));
                }
            }
            else
            {
                for (var i = 0; i < 16; i++)
                {
                    bits[i / 4] |= buffer[offset + i] << (8 * i);
                }
            }
            return new decimal(bits);
        }

        public static byte[] GetBytes(bool value)
        {
            return new byte[]
            {
                Convert.ToByte(value)
            };
        }

        public static byte[] GetBytes(short value, bool bigEndian = false)
        {
            if (bigEndian)
            {
                return new byte[]
                {
                    (byte)(value >> 8),
                    (byte)value
                };
            }
            else
            {
                return new byte[]
                {
                    (byte)value,
                    (byte)(value >> 8)
                };
            }
        }

        public static byte[] GetBytes(ushort value, bool bigEndian = false)
        {
            return GetBytes((short)value, bigEndian);
        }

        public static byte[] GetBytes(int value, bool isInt24 = false, bool bigEndian = false)
        {
            var size = isInt24 ? 3 : 4;
            var buffer = new byte[size];
            if (bigEndian)
            {
                for (var i = 0; i < size; i++)
                {
                    buffer[i] |= (byte)(value >> (8 * (size - i - 1)));
                }
            }
            else
            {
                for (var i = 0; i < size; i++)
                {
                    buffer[i] |= (byte)(value >> (8 * i));
                }
            }
            return buffer;
        }

        public static byte[] GetBytes(uint value, bool isUInt24 = false, bool bigEndian = false)
        {
            return GetBytes((int)value, isUInt24, bigEndian);
        }

        public static byte[] GetBytes(long value, bool bigEndian = false)
        {
            var buffer = new byte[8];
            if (bigEndian)
            {
                for (var i = 0; i < 8; i++)
                {
                    buffer[i] |= (byte)(value >> (8 * (7 - i)));
                }
            }
            else
            {
                for (var i = 0; i < 8; i++)
                {
                    buffer[i] |= (byte)(value >> (8 * i));
                }
            }
            return buffer;
        }

        public static byte[] GetBytes(ulong value, bool bigEndian = false)
        {
            return GetBytes((long)value, bigEndian);
        }

        public static byte[] GetBytes(float value, bool bigEndian = false)
        {
            return GetBytes(Convert.ToInt32(value), false, bigEndian);
        }

        public static byte[] GetBytes(double value, bool bigEndian = false)
        {
            return GetBytes(Convert.ToInt64(value), bigEndian);
        }

        public static byte[] GetBytes(decimal value, bool bigEndian = false)
        {
            var bits = decimal.GetBits(value);
            var buffer = new byte[16];
            if (bigEndian)
            {
                for (var i = 0; i < 16; i++)
                {
                    buffer[i] |= (byte)(bits[i / 4] >> (8 * ((15 - i) % 4)));
                }
            }
            else
            {
                for (var i = 0; i < 16; i++)
                {
                    buffer[i] |= (byte)(bits[i / 4] >> (8 * (i % 4)));
                }
            }
            return buffer;
        }
    }
}
