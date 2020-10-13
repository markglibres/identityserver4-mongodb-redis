
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users
{
    public class UserPasswordHasher<T> : PasswordHasher<T>
        where T: IdentityUser
    {
    }
}