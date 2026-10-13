using System;
using Water;

namespace Water
{

    public static class TokenBytes
    {
        public static string Hex(int bytes)
        {
            if (bytes < 8)
                bytes = 8;
            var buffer = new byte[bytes];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                rng.GetBytes(buffer);
            return HexText.FromBytes(buffer);
        }
    }

}
