using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer.Repositories;
using IdentityServer.Services;
using IdentityServer.Stores;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace IdentityServer
{
    public static class Startup
    {
        public static IIdentityServerBuilder AddMongoDbIdentityServer<TUser, TRole, TProfile>(
            this IServiceCollection services,
            Action<MongoOptions> mongoOptions,
            Action<IdentityServerOptions> identityOptions,
            Func<IServiceProvider, ICorsPolicyService> policy,
            Action<IIdentityServerBuilder> identityBuilder)
            where TUser : IdentityUser
            where TRole : IdentityRole
            where TProfile : ProfileService<TUser>
        {
            var mongoConfig = new MongoOptions();
            mongoOptions(mongoConfig);
            
            services.Configure<MongoOptions>(m =>
            {
                m.Database = mongoConfig.Database;
                m.ConnectionString = mongoConfig.ConnectionString;
            });
            
            services.AddTransient<ISeedService<TUser>, UserService<TUser>>();
            services.AddTransient<ISeedService<Client>, ClientService>();
            services.AddTransient<ISeedService<ApiResource>, ResourceService<ApiResource>>();
            services.AddTransient<ISeedService<ApiScope>, ResourceService<ApiScope>>();
            services.AddTransient<ISeedService<IdentityResource>, ResourceService<IdentityResource>>();

            services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();
            services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
            services.AddTransient(typeof(IRepository<>), typeof(MongoRepository<>));
            services.AddTransient<IUserService<TUser>, UserService<TUser>>();
            services.AddTransient<IClientService, ClientService>();

            var builder = services.AddIdentityServer(identityOptions);

            identityBuilder(builder);
            builder
                .AddAspNetIdentity<TUser>()
                .AddProfileService<TProfile>();
            services.AddSingleton(policy);

            return builder;
        }
        
        public static IIdentityServerBuilder AddMongoDbIdentityServer<TUser, TRole, TProfile>(
            this IServiceCollection services,
            Action<IdentityServerOptions> options,
            Func<IServiceProvider, ICorsPolicyService> policy,
            Action<IIdentityServerBuilder> identityBuilder)
            where TUser : IdentityUser
            where TRole : IdentityRole
            where TProfile : ProfileService<TUser>
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var configSection = configuration.GetSection("Identity:Mongo").Get<MongoOptions>();

            var builder = services.AddMongoDbIdentityServer<TUser, TRole, TProfile>(mongoOptions =>
            {
                mongoOptions.Database = configSection.Database;
                mongoOptions.ConnectionString = configSection.ConnectionString;
            }, options, policy, identityBuilder);

            return builder;
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

        // public static IIdentityServerBuilder AddRedisCaching(this IIdentityServerBuilder identityServerBuilder)
        // {
        //     identityServerBuilder
        //         .AddOperationalStore(options =>
        //         {
        //             options.RedisConnectionString = "---redis store connection string---";
        //             options.Db = 1;
        //         })
        // }

        private static void SetupDocument<T>()
        {
            BsonClassMap.RegisterClassMap<T>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
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
            if (identityResources?.Any() ?? false) identityServerBuilder.AddInMemoryIdentityResources(identityResources);
            
            return identityServerBuilder;
        }
    }
}