using System;
using IdentityServer.Management.Registration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Management
{
    public static class StartupClientInteraction
    {
        public static AuthenticationBuilder AddUserInteraction(this AuthenticationBuilder builder, Action<UserInteractionOptions> options)
        {
            var userInteractionOptions = new UserInteractionOptions();
            options(userInteractionOptions);

            var services = builder.Services;
            services.AddSingleton(provider => userInteractionOptions);
            services.AddTransient<IUserInteractionService, UserInteractionService>();

            return builder;

        }
    }

    public class UserInteractionOptions
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
    }
}
