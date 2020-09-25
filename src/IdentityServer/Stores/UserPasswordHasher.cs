using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Stores
{
    public class UserPasswordHasher<T> : PasswordHasher<T>
        where T : IdentityUser
    {
    }
}