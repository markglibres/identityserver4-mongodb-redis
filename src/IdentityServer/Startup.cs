using System;
using System.Linq;
using IdentityServer.Repositories;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;

namespace IdentityServer
{
    public static class Startup
    {
        // public static IIdentityServerBuilder AddMongoDbIdentityProvider<TUser, TRole, TProfile>(
        //     this IServiceCollection services,
        //     Action<MongoOptions> mongoOptions,
        //     Action<IdentityServerOptions> identityOptions,
        //     Func<IServiceProvider, ICorsPolicyService> policy,
        //     Action<IIdentityServerBuilder> identityBuilder)
        //     where TUser : IdentityUser
        //     where TRole : IdentityRole
        //     where TProfile : ProfileService<TUser>
        // {
        //     var mongoConfig = new MongoOptions();
        //     mongoOptions(mongoConfig);
        //
        //     services.Configure<MongoOptions>(m =>
        //     {
        //         m.Database = mongoConfig.Database;
        //         m.ConnectionString = mongoConfig.ConnectionString;
        //     });
        //
        //     services.AddTransient<ISeedService<TUser>, UserService<TUser>>();
        //     services.AddTransient<ISeedService<Client>, ClientService>();
        //     services.AddTransient<ISeedService<ApiResource>, ResourceService<ApiResource>>();
        //     services.AddTransient<ISeedService<ApiScope>, ResourceService<ApiScope>>();
        //     services.AddTransient<ISeedService<IdentityResource>, ResourceService<IdentityResource>>();
        //
        //     services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();
        //     services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
        //     services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
        //     services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
        //     services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
        //     services.AddTransient(typeof(IRepository<>), typeof(MongoRepository<>));
        //     services.AddTransient<IUserService<TUser>, UserService<TUser>>();
        //     services.AddTransient<IClientService, ClientService>();
        //
        //     var builder = services.AddIdentityServer(identityOptions);
        //
        //     identityBuilder(builder);
        //     builder
        //         .AddAspNetIdentity<TUser>()
        //         .AddProfileService<TProfile>();
        //     services.AddSingleton(policy);
        //
        //     return builder;
        // }
        //
        // public static IServiceCollection AddIdentityProvider<TUser, TRole>(
        //     this IServiceCollection services)
        //     where TUser : IdentityUser
        //     where TRole : IdentityRole
        // {
        //     services.AddTransient<ISeedService<TUser>, UserService<TUser>>();
        //     services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();
        //     services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
        //     services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
        //     services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
        //     services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
        //     services.AddTransient<IUserService<TUser>, UserService<TUser>>();
        //
        //     return services;
        // }
        //
        // public static IIdentityServerBuilder AddIdentityServer4<TUser, TProfile>(
        //     this IServiceCollection services,
        //     Action<IdentityServerOptions> identityOptions,
        //     Action<IIdentityServerBuilder> identityBuilder)
        //     where TUser : class
        //     where TProfile : ProfileService<TUser>
        // {
        //     services.AddTransient<ISeedService<Client>, ClientService>();
        //     services.AddTransient<ISeedService<ApiResource>, ResourceService<ApiResource>>();
        //     services.AddTransient<ISeedService<ApiScope>, ResourceService<ApiScope>>();
        //     services.AddTransient<ISeedService<IdentityResource>, ResourceService<IdentityResource>>();
        //
        //     services.AddTransient<IClientService, ClientService>();
        //
        //     var builder = services.AddIdentityServer(identityOptions);
        //
        //     identityBuilder(builder);
        //     return builder;
        // }
        //
        // public static IServiceCollection AddMongoDb(
        //     this IServiceCollection services,
        //     Action<MongoOptions> mongoOptions)
        // {
        //     var mongoConfig = new MongoOptions();
        //     mongoOptions(mongoConfig);
        //
        //     services.Configure<MongoOptions>(m =>
        //     {
        //         m.Database = mongoConfig.Database;
        //         m.ConnectionString = mongoConfig.ConnectionString;
        //     });
        //     services.AddTransient(typeof(IRepository<>), typeof(MongoRepository<>));
        //     return services;
        // }
        //
        // public static IIdentityServerBuilder AddMongoDbIdentityServer<TUser, TRole, TProfile>(
        //     this IServiceCollection services,
        //     Action<IdentityServerOptions> options,
        //     Func<IServiceProvider, ICorsPolicyService> policy,
        //     Action<IIdentityServerBuilder> identityBuilder)
        //     where TUser : IdentityUser
        //     where TRole : IdentityRole
        //     where TProfile : ProfileService<TUser>
        // {
        //     var provider = services.BuildServiceProvider();
        //     var configuration = provider.GetRequiredService<IConfiguration>();
        //     var configSection = configuration.GetSection("Identity:Mongo").Get<MongoOptions>();
        //
        //     var builder = services.AddMongoDbIdentityProvider<TUser, TRole, TProfile>(mongoOptions =>
        //     {
        //         mongoOptions.Database = configSection.Database;
        //         mongoOptions.ConnectionString = configSection.ConnectionString;
        //     }, options, policy, identityBuilder);
        //
        //     return builder;
        // }
        //
        // public static IIdentityServerBuilder AddMongoDbIdentityServer<TUser, TRole, TProfile>(
        //     this IServiceCollection services,
        //     Func<IServiceProvider, ICorsPolicyService> policy,
        //     Action<IIdentityServerBuilder> identityBuilder)
        //     where TUser : IdentityUser
        //     where TRole : IdentityRole
        //     where TProfile : ProfileService<TUser>
        // {
        //     var provider = services.BuildServiceProvider();
        //     var configuration = provider.GetRequiredService<IConfiguration>();
        //     var configSection = configuration.GetSection("Identity")?.Get<IdentityOptions>();
        //     
        //     var builder = services.AddMongoDbIdentityServer<TUser, TRole, TProfile>(options =>
        //     {
        //         options.IssuerUri = configSection?.Uri ?? "http://localhost:5000";
        //         
        //     }, policy, identityBuilder);
        //     return builder;
        // }
        //
        // public static IIdentityServerBuilder AddMongoDbIdentityServer<TUser, TRole, TProfile>(
        //     this IServiceCollection services,
        //     Action<IIdentityServerBuilder> identityBuilder)
        //     where TUser : IdentityUser
        //     where TRole : IdentityRole
        //     where TProfile : ProfileService<TUser>
        // {
        //     var provider = services.BuildServiceProvider();
        //     var defaultPolicy = new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
        //     {
        //         AllowAll = true
        //     };
        //     
        //     var builder = services.AddMongoDbIdentityServer<TUser, TRole, TProfile>(
        //         policy => defaultPolicy
        //         , identityBuilder);
        //     return builder;
        // }

        

        

        //
        // private static void SetupDocument<T>()
        // {
        //     BsonClassMap.RegisterClassMap<T>(map =>
        //     {
        //         map.AutoMap();
        //         map.SetIgnoreExtraElements(true);
        //     });
        // }
        //
        // public static IIdentityServerBuilder AddInMemoryClients(this IIdentityServerBuilder identityServerBuilder)
        // {
        //     identityServerBuilder.Services.AddTransient<IInMemorySettings<Client>, InMemorySettings<Client>>();
        //     var provider = identityServerBuilder.Services.BuildServiceProvider();
        //     var items = provider.GetService<ISeeder<Client>>()?.GetSeeds();
        //
        //     if (items == null) return identityServerBuilder;
        //
        //     identityServerBuilder.AddInMemoryClients(items);
        //     return identityServerBuilder;
        // }
        //
        // public static IIdentityServerBuilder AddInMemoryResources(this IIdentityServerBuilder identityServerBuilder)
        // {
        //     identityServerBuilder.Services.AddTransient<IInMemorySettings<Resource>, InMemorySettings<Resource>>();
        //     var provider = identityServerBuilder.Services.BuildServiceProvider();
        //     var apiResources = provider.GetService<ISeeder<ApiResource>>()?.GetSeeds().ToList();
        //     var apiScopes = provider.GetService<ISeeder<ApiScope>>()?.GetSeeds().ToList();
        //     var identityResources = provider.GetService<ISeeder<IdentityResource>>()?.GetSeeds().ToList();
        //
        //     if (apiResources?.Any() ?? false) identityServerBuilder.AddInMemoryApiResources(apiResources);
        //     if (apiScopes?.Any() ?? false) identityServerBuilder.AddInMemoryApiScopes(apiScopes);
        //     if (identityResources?.Any() ?? false)
        //         identityServerBuilder.AddInMemoryIdentityResources(identityResources);
        //
        //     return identityServerBuilder;
        // }
    }
}