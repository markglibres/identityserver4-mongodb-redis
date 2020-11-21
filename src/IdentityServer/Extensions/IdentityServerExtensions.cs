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
            var config = services.AddIdentityServerConfig();
            services.TryAddTransient(typeof(IIdentityRepository<>), typeof(IdentityMongoRepository<>));
            services.AddSingleton(setupPolicy);

            var builder = services
                .AddIdentityServer(options => { options.IssuerUri = config.Authority; })
                .AddMongoResources()
                .AddMongoClientStore();

            return builder;
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
            var config = identityServerBuilder.Services.AddIdentityServerConfig();
            identityServerBuilder
                .AddOperationalStore(options =>
                {
                    options.RedisConnectionString = config.Redis.ConnectionString;
                    options.Db = config.Redis.Db;
                }).AddRedisCaching(options =>
                {
                    options.RedisConnectionString = config.Redis.ConnectionString;
                    options.KeyPrefix = config.Redis.Prefix;
                });

            return identityServerBuilder;
        }
    }
}
