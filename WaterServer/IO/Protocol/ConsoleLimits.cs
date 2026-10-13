namespace Water.Host
{
    public static class ConsoleLimits
    {
        public const int MaxPayload = 1000000;
        public const byte ControlFloor = 16;

        public static bool IsControl(byte code)
        {
            return code >= ControlFloor;
        }

        public static bool PayloadOk(int size)
        {
            return size >= 0 && size <= MaxPayload;
        }
    }
}
