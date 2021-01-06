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
using IdentityServer.Management.Users.Abstractions;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer.Management
{
    public static class StartupIdentity
    {
        public static IMvcBuilder AddIdentityServerUserApi(this IMvcBuilder mvcBuilder)
        {
            var builder = mvcBuilder.AddApplicationPart(typeof(StartupIdentity).Assembly);

            return builder;
        }

        public static AuthenticationBuilder AddIdentityServerUserAuthentication(
            this AuthenticationBuilder builder,
            Action<IdentityAudienceConfig> audienceConfig = null)
        {
            var services = builder.Services;
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            IdentityAudienceConfig identityAudienceConfig = null;
            if (services.All(s => s.ServiceType != typeof(IOptions<IdentityAudienceConfig>)))
            {
                var config = configuration.GetSection("Identity:Audience");
                services.Configure<IdentityAudienceConfig>(config);
                identityAudienceConfig = config.Get<IdentityAudienceConfig>();
            }
            else
            {
                identityAudienceConfig = provider.GetRequiredService<IOptions<IdentityAudienceConfig>>().Value;
            }

            audienceConfig?.Invoke(identityAudienceConfig);

            const string introspectionScheme = "introspection";
            var authBuilder = builder
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
                {
                    jwtBearerOptions.Authority = identityAudienceConfig?.Authority;
                    jwtBearerOptions.Audience = identityAudienceConfig?.ClientId;
                    jwtBearerOptions.RequireHttpsMetadata = identityAudienceConfig?.RequireSsl ?? false;

                    if (identityAudienceConfig?.Introspection == null) return;
                    jwtBearerOptions.ForwardDefaultSelector = Selector.ForwardReferenceToken(introspectionScheme);
                });

            if (identityAudienceConfig?.Introspection != null)
                authBuilder.AddOAuth2Introspection(introspectionScheme, introspectionOptions =>
                {
                    introspectionOptions.Authority = identityAudienceConfig.Authority;
                    introspectionOptions.ClientId = identityAudienceConfig.ClientId;
                    introspectionOptions.ClientSecret = identityAudienceConfig.Introspection.ClientSecret;
                    introspectionOptions.DiscoveryPolicy = new DiscoveryPolicy
                    {
                        RequireHttps = identityAudienceConfig.RequireSsl
                    };
                });

            return builder;
        }

        [Obsolete("Use AddIdentityServerUser instead")]
        public static IIdentityServerBuilder AddResourceOwnerPassword<TUser, TRole>(
            this IIdentityServerBuilder builder)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            return builder.AddIdentityServerUser<TUser, TRole>();
        }

        public static IIdentityServerBuilder AddIdentityServerUser<TUser, TRole>(
            this IIdentityServerBuilder builder,
            Action<IdentityOptions> options = null)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            var services = builder.Services;

            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            if (services.All(s => s.ServiceType != typeof(IOptions<IdentityServerUserConfig>)))
            {
                var identityUserConfig = configuration.GetSection("Identity:Server:User");
                services.Configure<IdentityServerUserConfig>(identityUserConfig);
            }

            if (services.All(s => s.ServiceType != typeof(IOptions<SmtpConfig>)))
            {
                var smtpConfig = configuration.GetSection("Smtp");
                services.Configure<SmtpConfig>(smtpConfig);
            }

            services.AddIdentity<TUser, TRole>(identityOptions => { options?.Invoke(identityOptions); })
                .AddDefaultTokenProviders();
            services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
            services.AddTransient<IUserService<TUser>, UserService<TUser>>();
            services.AddTransient<IProfileService, ApplicationProfile>();

            services.AddMediatR(typeof(StartupIdentity).Assembly);
            services.AddTransient<IMapper, Mapper>();

            services.AddTransient<IApplicationEventPublisher, ApplicationEventService>();

            builder.AddAspNetIdentity<TUser>()
                .AddProfileService<ApplicationProfile>();

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

            services.AddTransient<IReturnUrlParser, ReturnUrlParser>();

            return builder;
        }

        internal static IIdentityServerBuilder AddIdentityUser<TUser>(this IIdentityServerBuilder builder)
            where TUser : IdentityUser
        {
            return builder.AddAspNetIdentity<TUser>();
        }

        private static IdentityAudienceConfig GetDefaulOptions()
        {
            var options = new IdentityAudienceConfig();
            return options;
        }
    }
}
