using System;

namespace Water
{

    public static class HexText
    {
        public static string FromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return "";

            const string hex = "0123456789abcdef";
            var chars = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                chars[i * 2] = hex[bytes[i] >> 4];
                chars[(i * 2) + 1] = hex[bytes[i] & 0x0F];
            }

            return new string(chars);
        }
    }

}
