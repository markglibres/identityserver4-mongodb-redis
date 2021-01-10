using IdentityServer.Authorization.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users.Authorization.Services
{
    public class ApplicationProfile : ProfileService<ApplicationUser>
    {
        public ApplicationProfile(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }
    }
}
