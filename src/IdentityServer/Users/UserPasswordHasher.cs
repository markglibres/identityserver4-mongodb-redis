using IdentityServer.Crypto;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users
{
    public class UserPasswordHasher<T> : PasswordHasher<T>
        where T : IdentityUser
    {
        public override string HashPassword(T user, string password)
        {
            return password.ToSha256();
        }

        public override PasswordVerificationResult VerifyHashedPassword(T user, string hashedPassword,
            string providedPassword)
        {
            var isMatched = hashedPassword.Equals(providedPassword.ToSha256());

            return isMatched ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}