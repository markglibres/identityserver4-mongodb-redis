using System;
using IdentityServer.Management.Registration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Management
{
    public static class StartupClientInteraction
    {
        public static AuthenticationBuilder AddUserInteraction(this AuthenticationBuilder builder)
        {
            var services = builder.Services;
            services.AddTransient<IUserInteractionService, UserInteractionService>();

            return builder;

        }
    }
}
