using System;

namespace Water
{

    public static class Utf8Clamp
    {
        public static byte[] Clip(string text, int maxBytes)
        {
            if (string.IsNullOrEmpty(text) || maxBytes <= 0)
                return Array.Empty<byte>();

            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            if (bytes.Length <= maxBytes)
                return bytes;

            var size = maxBytes;
            while (size > 0 && (bytes[size] & 0xC0) == 0x80)
                size--;

            var trimmed = new byte[size];
            Array.Copy(bytes, trimmed, size);
            return trimmed;
        }
    }

}
