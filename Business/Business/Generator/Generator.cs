using System;

namespace Core.Business.Generator
{
    /// <summary>
    /// Helper for generating random codes
    /// </summary>
    public static class Generator
    {
        private static readonly Random Random;

        static Generator()
        {
            Random = new Random();
        }

        /// <summary>
        /// Generate a random string
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns>A random code from the string: abcdefghijklmnopqrstuvwxyz0123456789</returns>
        public static string GenerateRandomCode(int minLength, int maxLength)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";

            string generatedCode = string.Empty;

            for (int counter = 0; Random.Next(minLength, maxLength) > counter; counter++)
            {
                generatedCode += alphabet[Random.Next(0, alphabet.Length)];
            }

            return generatedCode;
        }
    }
}
