using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Web
{
    public class ApplicationProfile : ProfileService<ApplicationUser>
    {
        public ApplicationProfile(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }
    }
}