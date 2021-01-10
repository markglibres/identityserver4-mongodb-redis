using IdentityServer.Common;
using IdentityServer4.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReturnUrlParser = IdentityServer.Users.Interactions.Application.ReturnUrlParser;

namespace IdentityServer.Users.Interactions
{
    public static class StartupIdentityUserInteraction
    {
        public static IMvcBuilder AddIdentityServerUserInteraction(this IMvcBuilder mvcBuilder)
        {
            var builder = mvcBuilder.AddApplicationPart(typeof(StartupIdentityUserInteraction).Assembly);
            return builder;
        }

        public static IIdentityServerBuilder AddIdentityServerUserInteraction(
            this IIdentityServerBuilder builder)
        {
            var services = builder.Services;

            services.AddMediatR(typeof(StartupIdentityUserInteraction).Assembly);
            services.TryAddTransient<IMapper, Mapper>();

            services.AddTransient<IReturnUrlParser, ReturnUrlParser>();

            return builder;
        }
    }
}
