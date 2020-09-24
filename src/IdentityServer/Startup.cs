using System;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public static class Startup
    {
        public static IIdentityServerBuilder AddInMemoryIdentityServer(this IServiceCollection services, 
            Action<IdentityServerOptions> options,
            Func<IServiceProvider, ICorsPolicyService> policy)
        {
            var provider = services.BuildServiceProvider();
            
            var builder = services.AddIdentityServer(options)
                .AddDeveloperSigningCredential() // use a valid signing cert in production
                .AddInMemoryIdentityResources(provider.GetService<IIdentityResource>().GetIdentityResources())    
                .AddInMemoryApiResources(provider.GetService<IApiResources>().GetApiResources())
                .AddInMemoryApiScopes(provider.GetService<IApiScopes>().GetApiScopes())
                .AddInMemoryClients(provider.GetService<IClients>().GetClients());
            
            services.AddSingleton(policy);

            return builder;
        }
    }
}