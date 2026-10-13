using System;

namespace Water
{

    public static class PasswordPolicy
    {
        public const int MinimumLength = 8;

        public static void Check(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < MinimumLength)
                throw new ArgumentException("Şifre en az 8 karakter olmalı.");
        }
    }

}
