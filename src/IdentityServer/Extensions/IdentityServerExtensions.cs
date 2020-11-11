using System;
using Identity.Common;
using IdentityServer.Services;
using IdentityServer.Services.Abstractions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        public static IIdentityServerBuilder AddResourceOwnerPassword<TUser, TRole>(
            this IIdentityServerBuilder builder)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            var services = builder.Services;
            services.AddIdentityUser<TUser, TRole>();
            builder.AddAspNetIdentity<TUser>();

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

        internal static IIdentityServerBuilder AddIdentityUser<TUser>(this IIdentityServerBuilder builder)
            where TUser : IdentityUser
        {
            return builder.AddAspNetIdentity<TUser>();
        }
    }
}