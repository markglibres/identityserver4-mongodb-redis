using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer.Repositories
{
    public static class Startup
    {
        public static IServiceCollection AddIdentityMongoDb(this IServiceCollection services, Action<IdentityMongoOptions> options)
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
        
        public static IServiceCollection AddIdentityMongoDb(this IServiceCollection services)
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
    }
}