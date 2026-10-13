namespace Water
{
    public static class WaterProduct
    {
        public const string Name = "Water";
        public const string Version = "1.0.0";
        public const string JoinMessage = "This server using Water Server!";
        public const string SupportedFrameworks = "net472, net48, net481, netstandard2.0, netstandard2.1, net6.0, net8.0, net10.0";
        public const int ProtocolVersion = 1;
        public const int TokenLength = 32;
    }

    public static class WaterRuntime
    {
        public static string Framework
        {
            get
            {
#if NET10_0_OR_GREATER
                return "net10.0";
#elif NET8_0_OR_GREATER
                return "net8.0";
#elif NET6_0_OR_GREATER
                return "net6.0";
#elif NET481
                return "net481";
#elif NET48
                return "net48";
#elif NET472
                return "net472";
#elif NETSTANDARD2_1
                return "netstandard2.1";
#elif NETSTANDARD2_0
                return "netstandard2.0";
#else
                return "unknown";
#endif
            }
        }
    }
}
