using IdentityServer.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Management.Users
{
    public class ApplicationProfile : ProfileService<ApplicationUser>
    {
        public ApplicationProfile(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }
    }
}