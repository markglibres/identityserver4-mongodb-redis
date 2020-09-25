using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public static class SeedClientsExtension
    {
        public static async Task SeedClients(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();

            var items = scope
                .ServiceProvider
                .GetService<ISeedClients>()
                ?.GetClients();
            
            if(items == null) return;
            
            var service = scope
                .ServiceProvider
                .GetService<IClientService>();

            foreach (var item in items)
            {
                await service.Create(item);
            }
        } 
    }
}