
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Authentication
{
    public class UserPasswordHasher<T> : PasswordHasher<T>
        where T: IdentityUser
    {
    }
}