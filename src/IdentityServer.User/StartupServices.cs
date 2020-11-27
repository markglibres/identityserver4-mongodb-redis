using System;
using System.Collections.Generic;
using System.Linq;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.Client;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IdentityServer.Management
{
    public static class Startup
    {
        public static IMvcBuilder AddIdentityServerUserApi(this IMvcBuilder mvcBuilder)
        {
            var builder = mvcBuilder.AddApplicationPart(typeof(Startup).Assembly);
            var services = mvcBuilder.Services;

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("IdentityUserV1", new OpenApiInfo
                {
                    Title = "Identity User",
                    Version = "v1"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://localhost:5000"),
                            TokenUrl = new Uri("http://localhost:5000/connect/token"),
                            RefreshUrl = new Uri("http://localhost:5000/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "myapi.access", "API access" },
                                { "myapi.user", "User Role" }
                            }
                        }
                    },
                    In = ParameterLocation.Header
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();

            return builder;
        }

        public static IIdentityServerBuilder AddIdentityServerAudience(
            this IIdentityServerBuilder builder)
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

            const string introspectionScheme = "introspection";
            var authBuilder = builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
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

            services.AddMediatR(typeof(Startup).Assembly);
            services.AddTransient<IMapper, Mapper>();

            services.AddTransient<IApplicationEventPublisher, ApplicationEventService>();

            builder.AddAspNetIdentity<TUser>()
                .AddProfileService<ApplicationProfile>();

            services.AddTransient<IFileReader, FileReader>();
            services.AddTransient<ITemplateParser, HandleBarsTemplateParser>();
            services.AddTransient<ITemplateProvider, EmbeddedResourceTemplateProvider>();
            services.AddTransient<IEmailTemplate, EmailTemplate>();
            services.AddTransient<IEmailer, SmtpEmailer>();

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
