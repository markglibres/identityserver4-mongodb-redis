using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Integration.Tests
{
    public abstract class ConfigurationSpecifications
    {
        protected ServiceProvider Services { get; }
        protected readonly Faker Faker;
        
        public ConfigurationSpecifications()
        {
            Services = Startup.ConfigureServices((services, configuration) => ConfigureServices(services, configuration));
            Faker = new Faker("en");
        }
        protected virtual IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) => services;
    }
}