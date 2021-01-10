using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Services.Abstractions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Seeders
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
                .GetServices<ISeeder<TSeed>>()
                ?.SelectMany(s => s.GetSeeds());

            if (items == null) return;

            var service = scope
                .ServiceProvider
                .GetRequiredService<ISeedService<TSeed>>();

            if (service == null) throw new NotImplementedException($"{typeof(ISeedService<TSeed>)} is not implemented");

            foreach (var item in items) await service.Create(item, cancellationToken);
        }

        public static async Task Initialize(this IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();

            if (scope.ServiceProvider.GetService<IInMemorySettings<Client>>() == null)
                await serviceProvider.Seed<Client>(cancellationToken);

            if (scope.ServiceProvider.GetService<IInMemorySettings<Resource>>() == null)
            {
                await serviceProvider.Seed<ApiResource>(cancellationToken);
                await serviceProvider.Seed<ApiScope>(cancellationToken);
                await serviceProvider.Seed<IdentityResource>(cancellationToken);
            }
        }

        public static async Task Initialize<TUser>(this IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
            where TUser : IdentityUser
        {
            using var scope = serviceProvider.CreateScope();
            await serviceProvider.Seed<TUser>(cancellationToken);
        }
    }
}
