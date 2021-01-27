using System;
using System.Linq;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityServer.Common;
using IdentityServer.Users.Interactions.Api;
using IdentityServer.Users.Interactions.Application.Abstractions;
using IdentityServer.Users.Interactions.Infrastructure;
using IdentityServer.Users.Interactions.Infrastructure.Config;
using IdentityServer.Users.Interactions.Infrastructure.Messaging;
using IdentityServer.Users.Interactions.Infrastructure.System;
using IdentityServer.Users.Interactions.Infrastructure.Templates;
using IdentityServer4.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ReturnUrlParser = IdentityServer.Users.Interactions.Infrastructure.ReturnUrlParser;

namespace IdentityServer.Users.Interactions
{
    public static class StartupUserInteraction
    {
        public static IMvcBuilder AddIdentityServerUserInteraction(this IMvcBuilder mvcBuilder,
            Action<IdentityServerUserInteractionConfig> userOptions = null,
            Action<SmtpConfig> smtpOptions = null)
        {
            var builder = mvcBuilder.AddApplicationPart(typeof(StartupUserInteraction).Assembly);

            var services = builder.Services;
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();

            if (services.All(s => s.ServiceType != typeof(IOptions<IdentityServerUserInteractionConfig>)))
            {
                var identityUserConfig = configuration.GetSection("Identity:UserManagement")
                    .Get<IdentityServerUserInteractionConfig>();
                services.Configure<IdentityServerUserInteractionConfig>(userConfig =>
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

            services.AddMediatR(typeof(StartupUserInteraction).Assembly);
            services.TryAddTransient<IMapper, Mapper>();

            services.AddTransient<IApplicationEventPublisher, ApplicationEventService>();

            services.AddTransient<IFileReader, FileReader>();
            services.AddTransient<ITemplateParser, HandleBarsTemplateParser>();
            services.AddTransient<ITemplateProvider, EmbeddedResourceTemplateProvider>();
            services.AddTransient<ITemplateProvider, LocalFileTemplateProvider>();
            services.AddTransient<IEmailTemplate, EmailTemplate>();
            services.AddTransient<IEmailer, SmtpEmailer>();
            services.AddTransient<IReturnUrlParser, ReturnUrlParser>();

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(Policies.UserManagement,
                    policyBuilder => { policyBuilder.RequireScope(Policies.Scopes.UserManagement); });
            });

            services.AddIdentityServerCommon();
            return builder;
        }
    }
}
