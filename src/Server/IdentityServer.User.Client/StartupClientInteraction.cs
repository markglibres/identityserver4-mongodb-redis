using IdentityServer.User.Client.Registration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer.User.Client
{
    public static class StartupClientInteraction
    {
        public static AuthenticationBuilder AddUserManagement(this AuthenticationBuilder builder,
            string authenticationScheme)
        {
            var services = builder.Services;
            var options = new UserInteractionServiceOptions {AuthenticationScheme = authenticationScheme};

            services.TryAddSingleton(provider => options);
            services.AddHttpClient<IUserInteractionService, UserInteractionService>();

            return builder;
        }
    }
}
