using System;
using System.Security.Claims;
using IdentityServer.Authorization.Services;
using IdentityServer.Authorization.Services.Abstractions;
using IdentityServer.Common.Repositories;
using IdentityServer.Common.Repositories.Abstractions;
using IdentityServer.Users.Interactions.Infrastructure.Config;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace IdentityServer.Authorization.Extensions
{
    public static class IdentityServerExtensions
    {
        public static IIdentityServerBuilder AddIdentityServerMongoDb(
            this IServiceCollection services,
            Action<IdentityServerOptions> identityServerOptions = null,
            Func<IServiceProvider, ICorsPolicyService> setupPolicy = null
        )
        {
            var config = services.AddIdentityServerConfig();
            services.TryAddTransient(typeof(IIdentityRepository<>), typeof(IdentityMongoRepository<>));

            var provider = services.BuildServiceProvider();
            var defaultPolicy = new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
            {
                AllowAll = true
            };
            services.AddSingleton(setupPolicy?.Invoke(provider) ?? defaultPolicy);

            var builder = services
                .AddIdentityServer(options =>
                {
                    options.IssuerUri = config.Authority;

                    var managementConfig = provider
                        .GetRequiredService<IOptions<IdentityServerUserInteractionConfig>>()
                        .Value;

                    if (!string.IsNullOrWhiteSpace(managementConfig.UserInteractionEndpoints.LoginUrl))
                        options.UserInteraction.LoginUrl = managementConfig.UserInteractionEndpoints.LoginUrl;

                    if (!string.IsNullOrWhiteSpace(managementConfig.UserInteractionEndpoints.LogoutUrl))
                        options.UserInteraction.LogoutUrl = managementConfig.UserInteractionEndpoints.LogoutUrl;

                    options.UserInteraction.LoginReturnUrlParameter = "returnUrl";

                    identityServerOptions?.Invoke(options);

                    options.Discovery.CustomEntries.Add("registration_endpoint",
                        $"~{managementConfig.UserInteractionEndpoints.CreateUser}");
                    options.Discovery.CustomEntries.Add("login_endpoint", $"~{options.UserInteraction.LoginUrl}");
                    options.Discovery.CustomEntries.Add("logout_endpoint", $"~{options.UserInteraction.LogoutUrl}");
                })
                .AddMongoResources()
                .AddMongoClientStore()
                .AddMongoUserStore();

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

        private static IIdentityServerBuilder AddMongoUserStore(this IIdentityServerBuilder identityServerBuilder)
        {
            BsonClassMap.RegisterClassMap<Claim>(cm =>
            {
                cm.SetIgnoreExtraElements(true);
                cm.MapMember(c => c.Issuer);
                cm.MapMember(c => c.OriginalIssuer);
                cm.MapMember(c => c.Properties);
                cm.MapMember(c => c.Subject);
                cm.MapMember(c => c.Type);
                cm.MapMember(c => c.Value);
                cm.MapMember(c => c.ValueType);
                cm.MapCreator(c => new Claim(c.Type, c.Value, c.ValueType, c.Issuer, c.OriginalIssuer, c.Subject));
            });

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