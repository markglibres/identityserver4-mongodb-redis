using System;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Domain.Extensions
{
    public static class SecretExtensions
    {
        public static string ToSha256(this string password)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}