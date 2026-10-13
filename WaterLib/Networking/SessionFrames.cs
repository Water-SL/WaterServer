using System;
using System.Text;

namespace Water
{
    public static class SessionFrames
    {
        public static bool HasToken(byte[] packet, string token)
        {
            if (packet == null || string.IsNullOrEmpty(token) || packet.Length <= token.Length)
                return false;

            var prefix = Encoding.ASCII.GetBytes(token);
            for (var i = 0; i < prefix.Length; i++)
            {
                if (packet[i] != prefix[i])
                    return false;
            }

            return true;
        }

        public static int PrefixLength(string token)
        {
            return string.IsNullOrEmpty(token) ? 0 : Encoding.ASCII.GetByteCount(token);
        }
    }
}
