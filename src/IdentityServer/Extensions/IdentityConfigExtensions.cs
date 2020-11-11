using IdentityServer.Repositories;
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


        internal static IdentityRedisOptions AddIdentityRedisConfig(this IServiceCollection services)
        {
            var config = new IdentityRedisOptions();

            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var redisSection = configuration.GetSection("Identity:Redis");
            services.Configure<IdentityRedisOptions>(redisSection);

            var configOptions = redisSection?.Get<IdentityRedisOptions>();

            return configOptions;
        }
    }
}