namespace Water.Host
{
    public static class LengthPrefix
    {
        public static int Read(byte[] length)
        {
            return length[0] | (length[1] << 8) | (length[2] << 16) | (length[3] << 24);
        }

        public static void Write(byte[] buffer, int value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
        }
    }
}
