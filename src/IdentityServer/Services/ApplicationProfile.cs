using IdentityServer.Users;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services
{
    public class ApplicationProfile : ProfileService<ApplicationUser>
    {
        public ApplicationProfile(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }
    }
}