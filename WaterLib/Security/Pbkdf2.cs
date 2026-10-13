using System;

namespace Water
{

    public static class Pbkdf2
    {
        public static byte[] Derive(string password, byte[] salt, int iterations)
        {
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password ?? "");
            using (var hmac = new System.Security.Cryptography.HMACSHA256(passwordBytes))
            {
                var block = new byte[salt.Length + 4];
                Buffer.BlockCopy(salt, 0, block, 0, salt.Length);
                block[salt.Length + 3] = 1;
                var u = hmac.ComputeHash(block);
                var t = new byte[u.Length];
                Buffer.BlockCopy(u, 0, t, 0, u.Length);
                for (var i = 1; i < iterations; i++)
                {
                    u = hmac.ComputeHash(u);
                    for (var j = 0; j < t.Length; j++)
                        t[j] ^= u[j];
                }

                return t;
            }
        }
    }

}
