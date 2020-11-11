using Identity.Common;
using Identity.Common.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class GrantTypesExtensions
    {
        public static IIdentityServerBuilder SeedUsers<TUser, TSeeder>(this IIdentityServerBuilder builder)
            where TUser : IdentityUser
            where TSeeder : class, ISeeder<TUser>
        {
            var services = builder.Services;
            services.AddIdentityUserSeeder<TUser, TSeeder>();
            return builder;
        }
    }
}