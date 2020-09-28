using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Services
{
    public static class SeedExtensions
    {
        public static async Task Seed<TSeed>(this IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
            where TSeed : class
        {
            using var scope = serviceProvider.CreateScope();

            var items = scope
                .ServiceProvider
                .GetService<ISeeder<TSeed>>()
                ?.GetSeeds();

            if (items == null) return;

            var service = scope
                .ServiceProvider
                .GetService<ISeedService<TSeed>>();

            if (service == null) throw new NotImplementedException($"{typeof(ISeedService<TSeed>)} is not implemented");

            foreach (var item in items) await service.Create(item, cancellationToken);
        }
        
        public static async Task Initialize<TUser>(this IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
            where TUser: IdentityUser
        {
            using var scope = serviceProvider.CreateScope();

            await serviceProvider.Seed<TUser>(cancellationToken);
            
            if (scope.ServiceProvider.GetService<IInMemorySettings<Client>>() == null)
            {
                await serviceProvider.Seed<Client>(cancellationToken);    
            }

            if (scope.ServiceProvider.GetService<IInMemorySettings<Resource>>() == null)
            {
                await serviceProvider.Seed<ApiResource>(cancellationToken);
                await serviceProvider.Seed<ApiScope>(cancellationToken);
                await serviceProvider.Seed<IdentityResource>(cancellationToken);    
            }
        }
    }
}