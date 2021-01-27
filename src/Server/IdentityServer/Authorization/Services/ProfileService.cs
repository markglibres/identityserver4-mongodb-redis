using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Authorization.Services
{
    public abstract class ProfileService<T> : IProfileService
        where T : IdentityUser
    {
        private readonly IdentityServerConfig _options;
        protected readonly UserManager<T> UserManager;

        protected ProfileService(UserManager<T> userManager,
            IOptions<IdentityServerConfig> options)
        {
            UserManager = userManager;
            _options = options.Value;
        }

        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await GetUser(context.Subject);
            context.IssuedClaims = (await GetClaims(user)).ToList();
        }

        public virtual async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await GetUser(context.Subject);
            context.IsActive = !_options.RequireConfirmedEmail || (user?.EmailConfirmed ?? false);
        }

        private async Task<T> GetUser(IPrincipal principal)
        {
            var userId = principal.GetSubjectId();
            return await UserManager.FindByIdAsync(userId);
        }

        protected virtual async Task<IList<Claim>> GetClaims(T user)
        {
            return await UserManager.GetClaimsAsync(user);
        }
    }
}