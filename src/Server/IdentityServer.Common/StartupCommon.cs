using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer.Common
{
    public static class StartupCommon
    {
        public static IServiceCollection AddIdentityServerCommon(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddTransient<IUrlBuilder, UrlBuilder>();

            return services;
        }
    }
}
