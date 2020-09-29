using IdentityServer.Services;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer.Extensions
{
    public static class SeedingExtensions
    {
        public static IIdentityServerBuilder SeedClients<TSeeder>(this IIdentityServerBuilder builder)
            where TSeeder : class, ISeeder<Client>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<Client>, ClientService>();
            services.AddTransient<ISeeder<Client>, TSeeder>();

            return builder;
        }
        
        public static IIdentityServerBuilder SeedApiResources<TSeeder>(this IIdentityServerBuilder builder)
            where TSeeder : class, ISeeder<ApiResource>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<ApiResource>, ResourceService<ApiResource>>();
            services.AddTransient<ISeeder<ApiResource>, TSeeder>();

            return builder;
        }
        
        public static IIdentityServerBuilder SeedApiScope<TSeeder>(this IIdentityServerBuilder builder)
            where TSeeder : class, ISeeder<ApiScope>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<ApiScope>, ResourceService<ApiScope>>();
            services.AddTransient<ISeeder<ApiScope>, TSeeder>();

            return builder;
        }
        
        public static IIdentityServerBuilder SeedIdentityResource<TSeeder>(this IIdentityServerBuilder builder)
            where TSeeder : class, ISeeder<IdentityResource>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<IdentityResource>, ResourceService<IdentityResource>>();
            services.AddTransient<ISeeder<IdentityResource>, TSeeder>();

            return builder;
        }
        
    }
}