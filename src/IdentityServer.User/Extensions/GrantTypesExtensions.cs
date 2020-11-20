using IdentityServer.Management.Users;
using IdentityServer.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer.Management.Extensions
{
    public static class GrantTypesExtensions
    {
        public static IIdentityServerBuilder SeedUsers<TUser, TSeeder>(this IIdentityServerBuilder builder)
            where TUser : IdentityUser
            where TSeeder : class, ISeeder<TUser>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<TUser>, UserService<TUser>>();
            services.AddSingleton<ISeeder<TUser>, TSeeder>();

            return builder;
        }
    }
}