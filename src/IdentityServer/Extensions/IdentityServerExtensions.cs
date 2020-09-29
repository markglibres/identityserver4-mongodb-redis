using System;
using IdentityServer.Repositories;
using IdentityServer.Services;
using IdentityServer.Stores;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class IdentityServerExtensions
    {
        public static IIdentityServerBuilder AddIdentityServerMongoDb(
            this IServiceCollection services,
            Func<IServiceProvider, ICorsPolicyService> setupPolicy)
        {
            var config = services.AddIdentityConfig();
            var builder =
                services.AddIdentityServerMongoDb(options => { options.IssuerUri = config.Authority; }, setupPolicy);

            return builder;
        }

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

        internal static IServiceCollection AddIdentityUser<TUser, TRole>(this IServiceCollection services)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            services.AddIdentityMongoDb();
            services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();
            services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
            services.AddTransient<IUserService<TUser>, UserService<TUser>>();

            return services;
        }

        internal static IIdentityServerBuilder AddIdentityUser<TUser>(this IIdentityServerBuilder builder)
            where TUser : IdentityUser
        {
            return builder.AddAspNetIdentity<TUser>();
        }
    }
}