using System;
using System.Linq;
using IdentityServer.Authorization;
using IdentityServer.Authorization.Abstractions;
using IdentityServer.Seeders;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class InMemoryExtensions
    {
        public static IIdentityServerBuilder AddIdentityServerInMemory(
            this IServiceCollection services,
            Action<IdentityServerOptions> setupIdentityOption,
            Func<IServiceProvider, ICorsPolicyService> setupPolicy)
        {
            services.AddTransient<IClientService, ClientService>();
            services.AddSingleton(setupPolicy);

            var builder = services
                .AddIdentityServer(setupIdentityOption)
                .AddInMemoryResources()
                .AddInMemoryClients();

            return builder;
        }

        public static IIdentityServerBuilder AddIdentityServerInMemory<TUser>(
            this IServiceCollection services,
            Action<IdentityServerOptions> setupIdentityOption,
            Func<IServiceProvider, ICorsPolicyService> setupPolicy)
            where TUser : IdentityUser
        {
            services.AddTransient<IClientService, ClientService>();
            services.AddSingleton(setupPolicy);

            var builder = services
                .AddIdentityServer(setupIdentityOption)
                .AddInMemoryResources()
                .AddInMemoryClients()
                .AddIdentityUser<TUser>();

            return builder;
        }

        internal static IIdentityServerBuilder AddInMemoryClients(this IIdentityServerBuilder identityServerBuilder)
        {
            identityServerBuilder.Services.AddTransient<IInMemorySettings<Client>, InMemorySettings<Client>>();
            var provider = identityServerBuilder.Services.BuildServiceProvider();
            var items = provider.GetService<ISeeder<Client>>()?.GetSeeds();

            if (items == null) return identityServerBuilder;

            identityServerBuilder.AddInMemoryClients(items);
            return identityServerBuilder;
        }

        internal static IIdentityServerBuilder AddInMemoryResources(this IIdentityServerBuilder identityServerBuilder)
        {
            identityServerBuilder.Services.AddTransient<IInMemorySettings<Resource>, InMemorySettings<Resource>>();
            var provider = identityServerBuilder.Services.BuildServiceProvider();
            var apiResources = provider.GetService<ISeeder<ApiResource>>()?.GetSeeds().ToList();
            var apiScopes = provider.GetService<ISeeder<ApiScope>>()?.GetSeeds().ToList();
            var identityResources = provider.GetService<ISeeder<IdentityResource>>()?.GetSeeds().ToList();

            if (apiResources?.Any() ?? false) identityServerBuilder.AddInMemoryApiResources(apiResources);
            if (apiScopes?.Any() ?? false) identityServerBuilder.AddInMemoryApiScopes(apiScopes);
            if (identityResources?.Any() ?? false)
                identityServerBuilder.AddInMemoryIdentityResources(identityResources);

            return identityServerBuilder;
        }
    }
}