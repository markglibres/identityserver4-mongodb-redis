using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Integration.Tests
{
    public static class Startup
    {
        public static ServiceProvider ConfigureServices(Action<IServiceCollection, IConfiguration> action = null)
        {
            var services = new ServiceCollection();
            var configBuilder = new ConfigurationBuilder();
            configBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables();
            var config = configBuilder.Build();

            ConfigureServices(services, config);
            action?.Invoke(services, config);

            return services.BuildServiceProvider();
        }

        public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity(configuration);
            //rvices.AddIdentityMongoDb();
            return services;
        }
    }
}