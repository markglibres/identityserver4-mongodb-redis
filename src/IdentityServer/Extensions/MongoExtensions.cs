using System;
using IdentityServer.Authorization;
using IdentityServer.Repositories;
using IdentityServer.Repositories.Abstractions;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization;

namespace IdentityServer.Extensions
{
    public static class MongoExtensions
    {
        internal static IServiceCollection AddIdentityMongoDb(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var config = provider.GetService<IConfiguration>();
            var configSection = config.GetSection("Identity:Mongo").Get<IdentityMongoOptions>();

            services.AddIdentityMongoDb(options =>
            {
                options.Database = configSection.Database;
                options.ConnectionString = configSection.ConnectionString;
            });

            return services;
        }
        
        internal static IServiceCollection AddIdentityMongoDb(this IServiceCollection services, Action<IdentityMongoOptions> options)
        {
            var mongoOptions = new IdentityMongoOptions();
            options(mongoOptions);
            
            services.Configure<IdentityMongoOptions>(o =>
            {
                o.Database = string.IsNullOrWhiteSpace(mongoOptions.Database) 
                    ? "IdentityServer" 
                    : mongoOptions.Database;
                
                o.ConnectionString = string.IsNullOrWhiteSpace(mongoOptions.ConnectionString)
                    ? "mongodb://root:foobar@localhost:27017/?readPreference=primaryPreferred&appname=IdentityServer"
                    : mongoOptions.ConnectionString;
            });
            
            services.TryAddTransient(typeof(IIdentityRepository<>), typeof(IdentityMongoRepository<>));

            return services;
        }
        
        internal static IIdentityServerBuilder AddMongoDbResources(this IIdentityServerBuilder identityServerBuilder)
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
        
        internal static IIdentityServerBuilder AddMongoDbClientStore(this IIdentityServerBuilder identityServerBuilder)
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
    }
}