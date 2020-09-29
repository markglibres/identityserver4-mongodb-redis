using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer.Extensions
{
    public static class IdentityConfigExtensions
    {
        internal static IdentityConfig AddIdentityConfig(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();

            var config = provider.GetService<IOptions<IdentityConfig>>();
            
            if (config != null) return config.Value;
            
            var configSection = configuration.GetSection("Identity");
            services.Configure<IdentityConfig>(configSection);

            return configSection.Get<IdentityConfig>();
        }
        
        
    }
}