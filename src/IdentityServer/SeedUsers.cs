using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public static class SeedUsersExtension
    {
        public static async Task SeedUsers<TUser>(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
            where TUser: IdentityUser
        {
            using var scope = serviceProvider.CreateScope();

            var users = scope
                .ServiceProvider
                .GetService<ISeedUsers<TUser>>()
                ?.GetUsers();
            
            if(users == null) return;
            
            var userService = scope
                .ServiceProvider
                .GetService<IUserService<TUser>>();

            foreach (var user in users)
            {
                await userService.Create(user, cancellationToken);
            }
        } 
    }
}