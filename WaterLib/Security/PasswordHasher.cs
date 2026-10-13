using System;
using System.Security.Cryptography;
using System.Text;

namespace Water
{
    public static class PasswordHasher
    {
        public static string Hash(string password, int iterations = 100000)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password is required.", nameof(password));
            if (iterations < 10000)
                iterations = 10000;

            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            var hash = Pbkdf2.Derive(password, salt, iterations);
            return "pbkdf2-sha256." + iterations.ToString() + "." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored))
                return false;

            var parts = stored.Split('.');
            if (parts.Length != 4 || parts[0] != "pbkdf2-sha256")
                return false;

            int iterations;
            if (!int.TryParse(parts[1], out iterations) || iterations < 10000)
                return false;

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            var actual = Pbkdf2.Derive(password, salt, iterations);
            return FixedEquals(actual, expected);
        }

        public static bool FixedEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
                return false;

            var diff = 0;
            for (var i = 0; i < left.Length; i++)
                diff |= left[i] ^ right[i];

            return diff == 0;
        }

        public static string CreateToken()
        {
            return TokenBytes.Hex(16);
        }
    }

    public interface IWaterPlugin
    {
        string Name { get; }

        void Enable();

        void Disable();
    }
}
