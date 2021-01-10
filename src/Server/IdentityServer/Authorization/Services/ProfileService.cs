using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Authorization.Services
{
    public abstract class ProfileService<T> : IProfileService
        where T : IdentityUser
    {
        protected readonly UserManager<T> UserManager;

        protected ProfileService(UserManager<T> userManager)
        {
            UserManager = userManager;
        }

        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await GetUser(context.Subject);
            context.IssuedClaims = GetClaims(user).ToList();
        }

        public virtual async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await GetUser(context.Subject);
            context.IsActive = user?.EmailConfirmed ?? false;
        }

        private async Task<T> GetUser(IPrincipal principal)
        {
            var userId = principal.GetSubjectId();
            return await UserManager.FindByIdAsync(userId);
        }

        protected virtual IEnumerable<Claim> GetClaims(T user)
        {
            return new List<Claim>();
        }
    }
}
