using System;
using System.Threading;
using System.Threading.Tasks;
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
    }
}