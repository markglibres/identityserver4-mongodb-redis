using IdentityServer.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Authorization
{
    public class ApplicationProfile : ProfileService<ApplicationUser>
    {
        public ApplicationProfile(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }
    }
}