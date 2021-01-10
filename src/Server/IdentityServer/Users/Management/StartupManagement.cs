using System;
using System.Linq;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityServer.Common;
using IdentityServer.Users.Management.Api;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Configs;
using IdentityServer.Users.Management.Infrastructure;
using IdentityServer.Users.Management.Infrastructure.Messaging;
using IdentityServer.Users.Management.Infrastructure.System;
using IdentityServer.Users.Management.Infrastructure.Templates;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IdentityServer.Users.Management
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
                var identityUserConfig = configuration.GetSection("Identity:UserManagement")
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
