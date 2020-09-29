using System;
using System.Dynamic;
using System.Linq;
using IdentityServer.Authentication;
using IdentityServer.Repositories;
using IdentityServer.Services;
using IdentityServer.Stores;
using IdentityServer.Web;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace IdentityServer.Authorization
{
    public static class Startup
    {
        public static IIdentityServerBuilder AddIdentityServerMongoDb(
            this IServiceCollection services,
            Action<IdentityServerOptions> setupIdentityOption,
            Func<IServiceProvider, ICorsPolicyService> setupPolicy)
        {
            services.AddIdentityConfig();
            services.AddIdentityMongoDb();
            services.AddTransient<IClientService, ClientService>();
            services.AddSingleton(setupPolicy);
            
            var builder = services
                .AddIdentityServer(setupIdentityOption)
                .AddMongoDbResources()
                .AddMongoDbClientStore();
            
            return builder;
        }
        
        public static IIdentityServerBuilder AddIdentityServerMongoDb(
            this IServiceCollection services,
            Func<IServiceProvider, ICorsPolicyService> setupPolicy)
        {
            var config = services.AddIdentityConfig();

            var builder = services.AddIdentityServerMongoDb(options =>
            {
                options.IssuerUri = config.Authority;
            }, setupPolicy);
            
            return builder;
        }

        public static IdentityConfig AddIdentityConfig(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();

            var config = provider.GetService<IOptions<IdentityConfig>>();
            
            if (config != null) return config.Value;
            
            var configSection = configuration.GetSection("Identity");
            services.Configure<IdentityConfig>(configSection);

            return configSection.Get<IdentityConfig>();
        }
        
        public static IIdentityServerBuilder AddResourceOwnerPassword<TUser, TRole>(
            this IIdentityServerBuilder builder)
            where TUser: IdentityUser
            where TRole: IdentityRole
        {
            var services = builder.Services;
            services.AddTransient<IClientService, ClientService>();
            services.AddIdentityUser<TUser, TRole>();
            
            builder.AddIdentityUser<TUser>();
            
            return builder;
        }
        
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
        where TUser: IdentityUser
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

        public static IIdentityServerBuilder AddIdentityUser<TUser>(this IIdentityServerBuilder builder)
            where TUser: IdentityUser
        {
            return builder.AddAspNetIdentity<TUser>();
        }
        
        public static IIdentityServerBuilder AddResourceOwnerPasswordUsers<TUser, TSeeder>(this IIdentityServerBuilder builder)
            where TUser: IdentityUser
            where TSeeder: class, ISeeder<TUser>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<TUser>, UserService<TUser>>();
            services.AddSingleton<ISeeder<TUser>, TSeeder>();
            return builder;
        }

        public static IIdentityServerBuilder AddClients<TSeeder>(this IIdentityServerBuilder builder)
            where TSeeder : class, ISeeder<Client>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<Client>, ClientService>();
            services.AddTransient<ISeeder<Client>, TSeeder>();

            return builder;
        }
        
        public static IIdentityServerBuilder AddApiResources<TSeeder>(this IIdentityServerBuilder builder)
            where TSeeder : class, ISeeder<ApiResource>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<ApiResource>, ResourceService<ApiResource>>();
            services.AddTransient<ISeeder<ApiResource>, TSeeder>();

            return builder;
        }
        
        public static IIdentityServerBuilder AddApiScope<TSeeder>(this IIdentityServerBuilder builder)
            where TSeeder : class, ISeeder<ApiScope>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<ApiScope>, ResourceService<ApiScope>>();
            services.AddTransient<ISeeder<ApiScope>, TSeeder>();

            return builder;
        }
        
        public static IIdentityServerBuilder AddIdentityResource<TSeeder>(this IIdentityServerBuilder builder)
            where TSeeder : class, ISeeder<IdentityResource>
        {
            var services = builder.Services;
            services.TryAddTransient<ISeedService<IdentityResource>, ResourceService<IdentityResource>>();
            services.AddTransient<ISeeder<IdentityResource>, TSeeder>();

            return builder;
        }
        
        public static IIdentityServerBuilder AddMongoDbResources(this IIdentityServerBuilder identityServerBuilder)
        {
            
            SetupDocument<IdentityResource>();
            SetupDocument<ApiResource>();
            SetupDocument<ApiScope>();
            SetupDocument<IdentityResources.OpenId>();
            SetupDocument<IdentityResources.Email>();
            SetupDocument<IdentityResources.Profile>();
            SetupDocument<IdentityResources.Address>();
            SetupDocument<IdentityResources.Phone>();

            identityServerBuilder.AddResourceStore<ResourceStore>();
            return identityServerBuilder;
        }
        
        public static IIdentityServerBuilder AddMongoDbClientStore(this IIdentityServerBuilder identityServerBuilder)
        {
            BsonClassMap.RegisterClassMap<Client>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
            identityServerBuilder.AddClientStore<ClientStore>();
            
            return identityServerBuilder;
        }
        
        private static void SetupDocument<T>()
        {
            BsonClassMap.RegisterClassMap<T>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }
        
        public static IIdentityServerBuilder AddRedisCaching(
            this IIdentityServerBuilder identityServerBuilder,
            Action<IdentityRedisOptions> redisOptions)
        {
            var config = new IdentityRedisOptions();
            redisOptions?.Invoke(config);

            identityServerBuilder
                .AddOperationalStore(options =>
                {
                    options.RedisConnectionString = config.ConnectionString;
                    options.Db = config.Db;
                }).AddRedisCaching(options =>
                {
                    options.RedisConnectionString = config.ConnectionString;
                    options.KeyPrefix = config.Prefix;
                });

            return identityServerBuilder;
        }

        public static IIdentityServerBuilder AddRedisCaching(this IIdentityServerBuilder identityServerBuilder)
        {
            var config = new IdentityRedisOptions();

            var provider = identityServerBuilder.Services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var configSection = configuration.GetSection("Identity:Redis")?.Get<IdentityRedisOptions>();

            if (configSection != null)
            {
                if (!string.IsNullOrWhiteSpace(configSection.ConnectionString))
                    config.ConnectionString = configSection.ConnectionString;

                if (configSection.Db != 0) config.Db = configSection.Db;

                if (!string.IsNullOrWhiteSpace(configSection.Prefix)) config.Prefix = configSection.Prefix;
            }

            identityServerBuilder
                .AddOperationalStore(options =>
                {
                    options.RedisConnectionString = config.ConnectionString;
                    options.Db = config.Db;
                }).AddRedisCaching(options =>
                {
                    options.RedisConnectionString = config.ConnectionString;
                    options.KeyPrefix = config.Prefix;
                });

            return identityServerBuilder;
        }
        
        public static IIdentityServerBuilder AddInMemoryClients(this IIdentityServerBuilder identityServerBuilder)
        {
            identityServerBuilder.Services.AddTransient<IInMemorySettings<Client>, InMemorySettings<Client>>();
            var provider = identityServerBuilder.Services.BuildServiceProvider();
            var items = provider.GetService<ISeeder<Client>>()?.GetSeeds();
        
            if (items == null) return identityServerBuilder;
        
            identityServerBuilder.AddInMemoryClients(items);
            return identityServerBuilder;
        }
        
        public static IIdentityServerBuilder AddInMemoryResources(this IIdentityServerBuilder identityServerBuilder)
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