using System;
using System.Linq;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.Client;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Abstractions;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Interactions.Infrastructure.Config;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IdentityServer.Users.Authorization
{
    public static class StartupIdentityUser
    {
        public static AuthenticationBuilder AddIdentityServerUserAuthorization(
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
            return builder.AddIdentityServerUserAuthorization<TUser, TRole>();
        }

        public static IIdentityServerBuilder AddIdentityServerUserAuthorization<TUser, TRole>(
            this IIdentityServerBuilder builder,
            Action<IdentityOptions> options = null)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            var services = builder.Services;

            services.AddIdentity<TUser, TRole>(identityOptions => { options?.Invoke(identityOptions); })
                .AddDefaultTokenProviders();
            services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserClaimStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
            services.AddTransient<IUserService<TUser>, UserService<TUser>>();
            services.AddTransient<IProfileService, ApplicationProfile>();

            services.AddMediatR(typeof(StartupIdentityUser).Assembly);
            services.TryAddTransient<IMapper, Mapper>();

            builder.AddAspNetIdentity<TUser>()
                .AddProfileService<ApplicationProfile>();

            return builder;
        }
    }
}