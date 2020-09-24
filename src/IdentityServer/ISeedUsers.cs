using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer
{
    public interface ISeedUsers<out T>
        where T: IdentityUser
    {
        IEnumerable<T> GetUsers();
    }
}