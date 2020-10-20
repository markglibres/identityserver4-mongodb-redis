using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Integration.Tests
{
    public abstract class ConfigurationSpecifications
    {
        protected ServiceProvider Services { get; }
        
        public ConfigurationSpecifications()
        {
            Services = Startup.ConfigureServices((services, configuration) => ConfigureServices(services, configuration));
        }
        protected virtual IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) => services;
    }
}