using System;
using IdentityServer.Repositories;
using IdentityServer.Services;
using IdentityServer.Stores;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace IdentityServer
{
    public static class Startup
    {
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

            services.Configure<MongoOptions>(configuration.GetSection("Identity:Mongo"));

            services.AddTransient<ISeedService<TUser>, UserService<TUser>>();
            services.AddTransient<ISeedService<Client>, ClientService>();

            services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();
            services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
            services.AddTransient(typeof(IRepository<>), typeof(MongoRepository<>));
            services.AddTransient<IUserService<TUser>, UserService<TUser>>();
            services.AddTransient<IClientService, ClientService>();

            var builder = services.AddIdentityServer(options);

            identityBuilder(builder);
            builder
                .AddAspNetIdentity<TUser>()
                .AddProfileService<TProfile>();
            services.AddSingleton(policy);

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

        public static IIdentityServerBuilder AddInMemoryClients(this IIdentityServerBuilder identityServerBuilder)
        {
            var provider = identityServerBuilder.Services.BuildServiceProvider();
            var items = provider.GetService<ISeeder<Client>>()?.GetSeeds();

            if (items == null) return identityServerBuilder;

            identityServerBuilder.AddInMemoryClients(items);
            return identityServerBuilder;
        }
    }
}