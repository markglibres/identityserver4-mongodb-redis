using IdentityServer.Authorization;
using IdentityServer.Authorization.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Users.Authorization.Services
{
    public class ApplicationProfile : ProfileService<ApplicationUser>
    {
        public ApplicationProfile(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityServerConfig> options)
            : base(userManager, options)
        {
        }
    }
}