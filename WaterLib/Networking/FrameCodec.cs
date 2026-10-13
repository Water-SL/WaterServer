using System;
using System.Text;

namespace Water
{
    public static class FrameCodec
    {
        public static readonly byte[] Magic = { (byte)'W', (byte)'T', (byte)'R', (byte)'1' };

        public static int Encode(RelayFrame frame, byte[] buffer, int start)
        {
            if (buffer == null || start < 0 || buffer.Length - start < 32)
                return -1;

            var offset = start;
            buffer[offset++] = Magic[0];
            buffer[offset++] = Magic[1];
            buffer[offset++] = Magic[2];
            buffer[offset++] = Magic[3];
            WriteUInt16(buffer, ref offset, (ushort)WaterProduct.ProtocolVersion);
            WriteInt64(buffer, ref offset, frame.Tick);
            WriteInt64(buffer, ref offset, frame.Timestamp);
            WriteFloat(buffer, ref offset, frame.Alpha);
            buffer[offset++] = frame.Extrapolating ? (byte)1 : (byte)0;
            buffer[offset++] = 0;

            if (!WritePlayers(frame, buffer, ref offset))
                return -1;
            if (!WriteToys(frame, buffer, ref offset))
                return -1;
            if (!WriteItems(frame, buffer, ref offset))
                return -1;

            return offset - start;
        }

        public static bool TryDecode(byte[] buffer, int start, int length, RelayFrame frame)
        {
            frame.Clear();
            if (buffer == null || length < 28 || start < 0 || start + length > buffer.Length)
                return false;
            if (buffer[start] != Magic[0] || buffer[start + 1] != Magic[1] || buffer[start + 2] != Magic[2] || buffer[start + 3] != Magic[3])
                return false;

            var offset = start + 4;
            var end = start + length;
            var version = ReadUInt16(buffer, ref offset);
            if (version != WaterProduct.ProtocolVersion || offset > end)
                return false;

            frame.Tick = ReadInt64(buffer, ref offset);
            frame.Timestamp = ReadInt64(buffer, ref offset);
            frame.Alpha = ReadFloat(buffer, ref offset);
            if (offset >= length)
                return false;

            frame.Extrapolating = buffer[offset++] == 1;
            offset++;

            if (!ReadPlayers(buffer, end, ref offset, frame))
                return false;
            if (!ReadToys(buffer, end, ref offset, frame))
                return false;
            if (!ReadItems(buffer, end, ref offset, frame))
                return false;

            return true;
        }

        private static bool WritePlayers(RelayFrame frame, byte[] buffer, ref int offset)
        {
            if (offset + 2 > buffer.Length)
                return false;

            WriteUInt16(buffer, ref offset, (ushort)frame.PlayerCount);
            for (var i = 0; i < frame.PlayerCount; i++)
            {
                var sample = frame.Players[i];
                var name = sample.Name ?? "";
                var nameBytes = Encoding.UTF8.GetBytes(name);
                if (nameBytes.Length > 64)
                {
                    var trimmed = new byte[64];
                    Array.Copy(nameBytes, trimmed, 64);
                    nameBytes = trimmed;
                }

                var needed = 28 + 1 + nameBytes.Length;
                if (offset + needed > buffer.Length)
                    return false;

                WriteInt32(buffer, ref offset, sample.Id);
                WriteFloat(buffer, ref offset, sample.X);
                WriteFloat(buffer, ref offset, sample.Y);
                WriteFloat(buffer, ref offset, sample.Z);
                WriteFloat(buffer, ref offset, sample.Yaw);
                WriteFloat(buffer, ref offset, sample.Pitch);
                WriteInt32(buffer, ref offset, sample.Held);
                buffer[offset++] = (byte)nameBytes.Length;
                Buffer.BlockCopy(nameBytes, 0, buffer, offset, nameBytes.Length);
                offset += nameBytes.Length;
            }

            return true;
        }

        private static bool WriteToys(RelayFrame frame, byte[] buffer, ref int offset)
        {
            if (offset + 2 > buffer.Length)
                return false;

            WriteUInt16(buffer, ref offset, (ushort)frame.ToyCount);
            for (var i = 0; i < frame.ToyCount; i++)
            {
                if (offset + 20 > buffer.Length)
                    return false;

                var sample = frame.Toys[i];
                WriteInt32(buffer, ref offset, sample.Id);
                WriteFloat(buffer, ref offset, sample.X);
                WriteFloat(buffer, ref offset, sample.Y);
                WriteFloat(buffer, ref offset, sample.Z);
                WriteFloat(buffer, ref offset, sample.Yaw);
            }

            return true;
        }

        private static bool WriteItems(RelayFrame frame, byte[] buffer, ref int offset)
        {
            if (offset + 2 > buffer.Length)
                return false;

            WriteUInt16(buffer, ref offset, (ushort)frame.ItemCount);
            for (var i = 0; i < frame.ItemCount; i++)
            {
                if (offset + 24 > buffer.Length)
                    return false;

                var sample = frame.Items[i];
                WriteInt32(buffer, ref offset, sample.Id);
                WriteInt32(buffer, ref offset, sample.Type);
                WriteFloat(buffer, ref offset, sample.X);
                WriteFloat(buffer, ref offset, sample.Y);
                WriteFloat(buffer, ref offset, sample.Z);
                WriteInt32(buffer, ref offset, sample.Owner);
            }

            return true;
        }

        private static bool ReadPlayers(byte[] buffer, int length, ref int offset, RelayFrame frame)
        {
            if (offset + 2 > length)
                return false;

            var count = ReadUInt16(buffer, ref offset);
            for (var i = 0; i < count; i++)
            {
                if (offset + 29 > length)
                    return false;

                var sample = new PlayerSample();
                sample.Id = ReadInt32(buffer, ref offset);
                sample.X = ReadFloat(buffer, ref offset);
                sample.Y = ReadFloat(buffer, ref offset);
                sample.Z = ReadFloat(buffer, ref offset);
                sample.Yaw = ReadFloat(buffer, ref offset);
                sample.Pitch = ReadFloat(buffer, ref offset);
                sample.Held = ReadInt32(buffer, ref offset);
                var nameLength = buffer[offset++];
                if (offset + nameLength > length)
                    return false;

                sample.Name = nameLength == 0 ? "" : Encoding.UTF8.GetString(buffer, offset, nameLength);
                offset += nameLength;
                Add(frame, sample);
            }

            return true;
        }

        private static bool ReadToys(byte[] buffer, int length, ref int offset, RelayFrame frame)
        {
            if (offset + 2 > length)
                return false;

            var count = ReadUInt16(buffer, ref offset);
            for (var i = 0; i < count; i++)
            {
                if (offset + 20 > length)
                    return false;

                var sample = new ToySample();
                sample.Id = ReadInt32(buffer, ref offset);
                sample.X = ReadFloat(buffer, ref offset);
                sample.Y = ReadFloat(buffer, ref offset);
                sample.Z = ReadFloat(buffer, ref offset);
                sample.Yaw = ReadFloat(buffer, ref offset);
                Add(frame, sample);
            }

            return true;
        }

        private static bool ReadItems(byte[] buffer, int length, ref int offset, RelayFrame frame)
        {
            if (offset + 2 > length)
                return false;

            var count = ReadUInt16(buffer, ref offset);
            for (var i = 0; i < count; i++)
            {
                if (offset + 24 > length)
                    return false;

                var sample = new ItemSample();
                sample.Id = ReadInt32(buffer, ref offset);
                sample.Type = ReadInt32(buffer, ref offset);
                sample.X = ReadFloat(buffer, ref offset);
                sample.Y = ReadFloat(buffer, ref offset);
                sample.Z = ReadFloat(buffer, ref offset);
                sample.Owner = ReadInt32(buffer, ref offset);
                Add(frame, sample);
            }

            return true;
        }

        private static void Add(RelayFrame frame, PlayerSample sample)
        {
            if (frame.PlayerCount == frame.Players.Length)
                Array.Resize(ref frame.Players, frame.Players.Length * 2);
            frame.Players[frame.PlayerCount++] = sample;
        }

        private static void Add(RelayFrame frame, ToySample sample)
        {
            if (frame.ToyCount == frame.Toys.Length)
                Array.Resize(ref frame.Toys, frame.Toys.Length * 2);
            frame.Toys[frame.ToyCount++] = sample;
        }

        private static void Add(RelayFrame frame, ItemSample sample)
        {
            if (frame.ItemCount == frame.Items.Length)
                Array.Resize(ref frame.Items, frame.Items.Length * 2);
            frame.Items[frame.ItemCount++] = sample;
        }

        private static void WriteUInt16(byte[] buffer, ref int offset, ushort value)
        {
            buffer[offset++] = (byte)value;
            buffer[offset++] = (byte)(value >> 8);
        }

        private static void WriteInt32(byte[] buffer, ref int offset, int value)
        {
            buffer[offset++] = (byte)value;
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)(value >> 16);
            buffer[offset++] = (byte)(value >> 24);
        }

        private static void WriteInt64(byte[] buffer, ref int offset, long value)
        {
            for (var i = 0; i < 8; i++)
                buffer[offset++] = (byte)(value >> (8 * i));
        }

        private static void WriteFloat(byte[] buffer, ref int offset, float value)
        {
            var bits = new FloatBits { Float = value };
            WriteInt32(buffer, ref offset, bits.Int);
        }

        private static ushort ReadUInt16(byte[] buffer, ref int offset)
        {
            var value = (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
            offset += 2;
            return value;
        }

        private static int ReadInt32(byte[] buffer, ref int offset)
        {
            var value = buffer[offset]
                | (buffer[offset + 1] << 8)
                | (buffer[offset + 2] << 16)
                | (buffer[offset + 3] << 24);
            offset += 4;
            return value;
        }

        private static long ReadInt64(byte[] buffer, ref int offset)
        {
            long value = 0;
            for (var i = 0; i < 8; i++)
                value |= (long)buffer[offset++] << (8 * i);

            return value;
        }

        private static float ReadFloat(byte[] buffer, ref int offset)
        {
            var bits = new FloatBits { Int = ReadInt32(buffer, ref offset) };
            return bits.Float;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct FloatBits
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public float Float;

            [System.Runtime.InteropServices.FieldOffset(0)]
            public int Int;
        }
    }
}
