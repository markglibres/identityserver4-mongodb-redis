using Microsoft.AspNetCore.Identity;

namespace IdentityServer
{
    public class UserPasswordHasher<T> : PasswordHasher<T>
        where T: IdentityUser
    {
    }
}