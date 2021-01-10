using System.Linq;
using IdentityServer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer.Extensions
{
    public static class IdentityConfigExtensions
    {
        internal static IdentityServerConfig AddIdentityServerConfig(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            IdentityServerConfig identityServerServerConfig = null;
            if (services.All(s => s.ServiceType != typeof(IdentityServerConfig)))
            {
                var config = configuration.GetSection("Identity:Server");
                services.Configure<IdentityServerConfig>(config);
                identityServerServerConfig = config.Get<IdentityServerConfig>();
            }
            else
            {
                identityServerServerConfig = provider.GetRequiredService<IOptions<IdentityServerConfig>>().Value;
            }

            return identityServerServerConfig;
        }

    }
}
