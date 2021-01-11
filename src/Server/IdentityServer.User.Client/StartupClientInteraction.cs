using System;
using IdentityServer.User.Client.Registration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer.User.Client
{
    public static class StartupClientInteraction
    {
        public static AuthenticationBuilder AddUserManagement(this AuthenticationBuilder builder,
            Action<UserInteractionServiceOptions> options = null)
        {
            var services = builder.Services;
            var defaultOptions = new UserInteractionServiceOptions
            {
                AuthenticationScheme = "oidc"
            };
            options?.Invoke(defaultOptions);

            services.TryAddSingleton(provider => defaultOptions);
            services.AddHttpClient<IUserInteractionService, UserInteractionService>();

            return builder;
        }
    }
}
