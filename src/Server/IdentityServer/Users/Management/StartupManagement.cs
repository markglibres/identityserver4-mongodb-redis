using System;
using System.Linq;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.Client;
using IdentityServer.Management.Api;
using IdentityServer.Management.Application.Abstractions;
using IdentityServer.Management.Common;
using IdentityServer.Management.Infrastructure;
using IdentityServer.Management.Infrastructure.Config;
using IdentityServer.Management.Infrastructure.Messaging;
using IdentityServer.Management.Infrastructure.System;
using IdentityServer.Management.Infrastructure.Templates;
using IdentityServer.Management.Users;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IdentityServer.Management
{
    public static class StartupManagement
    {
        public static IMvcBuilder AddIdentityServerUserManagement(this IMvcBuilder mvcBuilder,
            Action<IdentityServerUserManagementConfig> userOptions = null,
            Action<SmtpConfig> smtpOptions = null)
        {
            var builder = mvcBuilder.AddApplicationPart(typeof(StartupManagement).Assembly);

            var services = builder.Services;
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();

            if (services.All(s => s.ServiceType != typeof(IOptions<IdentityServerUserManagementConfig>)))
            {
                var identityUserConfig = configuration.GetSection("Identity:User:Management")
                    .Get<IdentityServerUserManagementConfig>();
                services.Configure<IdentityServerUserManagementConfig>(userConfig =>
                {
                    Mapper.MapNotNullProperties(identityUserConfig, userConfig);
                    userOptions?.Invoke(userConfig);
                });
            }

            if (services.All(s => s.ServiceType != typeof(IOptions<SmtpConfig>)))
            {
                var smtpConfig = configuration.GetSection("Smtp");
                services.Configure<SmtpConfig>(smtpConfig);
            }

            services.AddMediatR(typeof(StartupManagement).Assembly);
            services.TryAddTransient<IMapper, Mapper>();

            services.AddTransient<IApplicationEventPublisher, ApplicationEventService>();

            services.AddTransient<IFileReader, FileReader>();
            services.AddTransient<ITemplateParser, HandleBarsTemplateParser>();
            services.AddTransient<ITemplateProvider, EmbeddedResourceTemplateProvider>();
            services.AddTransient<IEmailTemplate, EmailTemplate>();
            services.AddTransient<IEmailer, SmtpEmailer>();

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(Policies.UserManagement,
                    policyBuilder => { policyBuilder.RequireScope(Policies.Scopes.UserManagement); });
            });

            return builder;
        }

    }
}
