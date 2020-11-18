using System;
using IdentityServer.Repositories;
using IdentityServer.Repositories.Abstractions;
using IdentityServer.Services;
using IdentityServer.Services.Abstractions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace IdentityServer.Extensions
{
    public static class IdentityServerExtensions
    {
        public static IIdentityServerBuilder AddIdentityServerMongoDb(
            this IServiceCollection services,
            Func<IServiceProvider, ICorsPolicyService> setupPolicy)
        {
            var config = services.AddIdentityConfig();
            services.AddIdentityMongoDb();
            services.AddSingleton(setupPolicy);

            var builder = services
                .AddIdentityServer(options => { options.IssuerUri = config.Authority; })
                .AddMongoResources()
                .AddMongoClientStore();

            return builder;
        }

        public static IServiceCollection AddIdentityMongoDb(
            this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();

            var config = provider.GetService<IOptions<IdentityMongoOptions>>();

            if (!string.IsNullOrWhiteSpace(config?.Value?.ConnectionString)) return services;

            var mongoSection = configuration.GetSection("Identity:Mongo");

            services.Configure<IdentityMongoOptions>(mongoSection);
            mongoSection.Get<IdentityMongoOptions>();

            services.TryAddTransient(typeof(IIdentityRepository<>), typeof(IdentityMongoRepository<>));

            return services;
        }

        private static IIdentityServerBuilder AddMongoResources(this IIdentityServerBuilder identityServerBuilder)
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

        private static void SetupDocument<T>()
        {
            BsonClassMap.RegisterClassMap<T>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }

        private static IIdentityServerBuilder AddMongoClientStore(this IIdentityServerBuilder identityServerBuilder)
        {
            BsonClassMap.RegisterClassMap<Client>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
            identityServerBuilder.AddClientStore<ClientStore>();
            identityServerBuilder.Services.TryAddTransient<IClientService, ClientService>();

            return identityServerBuilder;
        }

        public static IIdentityServerBuilder AddRedisCache(this IIdentityServerBuilder identityServerBuilder)
        {
            var config = identityServerBuilder.Services.AddIdentityRedisConfig();
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
    }
}