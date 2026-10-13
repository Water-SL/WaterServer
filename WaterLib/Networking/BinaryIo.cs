using System;

namespace Water
{

    public static class BinaryIo
    {
        public static void WriteUInt16(byte[] buffer, ref int offset, ushort value)
        {
            buffer[offset++] = (byte)value;
            buffer[offset++] = (byte)(value >> 8);
        }

        public static void WriteInt32(byte[] buffer, ref int offset, int value)
        {
            buffer[offset++] = (byte)value;
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)(value >> 16);
            buffer[offset++] = (byte)(value >> 24);
        }

        public static void WriteInt64(byte[] buffer, ref int offset, long value)
        {
            for (var i = 0; i < 8; i++)
                buffer[offset++] = (byte)(value >> (8 * i));
        }

        public static void WriteFloat(byte[] buffer, ref int offset, float value)
        {
            var bits = new FloatBits { Float = value };
            WriteInt32(buffer, ref offset, bits.Int);
        }

        public static ushort ReadUInt16(byte[] buffer, ref int offset)
        {
            var value = (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
            offset += 2;
            return value;
        }

        public static int ReadInt32(byte[] buffer, ref int offset)
        {
            var value = buffer[offset]
                | (buffer[offset + 1] << 8)
                | (buffer[offset + 2] << 16)
                | (buffer[offset + 3] << 24);
            offset += 4;
            return value;
        }

        public static long ReadInt64(byte[] buffer, ref int offset)
        {
            long value = 0;
            for (var i = 0; i < 8; i++)
                value |= (long)buffer[offset++] << (8 * i);
            return value;
        }

        public static float ReadFloat(byte[] buffer, ref int offset)
        {
            var bits = new FloatBits { Int = ReadInt32(buffer, ref offset) };
            return bits.Float;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct FloatBits
        {
            [System.Runtime.InteropServices.FieldOffset(0)] public float Float;
            [System.Runtime.InteropServices.FieldOffset(0)] public int Int;
        }
    }

}
