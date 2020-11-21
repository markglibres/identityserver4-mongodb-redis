using System;
using AngleSharp;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.Client;
using IdentityServer.Management.Application.Abstractions;
using IdentityServer.Management.Common;
using IdentityServer.Management.Infrastructure;
using IdentityServer.Management.Infrastructure.Abstractions;
using IdentityServer.Management.Infrastructure.Messaging;
using IdentityServer.Management.Infrastructure.System;
using IdentityServer.Management.Infrastructure.Templates;
using IdentityServer.Management.Users;
using IdentityServer.Management.Users.Abstractions;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace IdentityServer.Management
{
    public static class Startup
    {
        public static IMvcBuilder AddIdentityServerUserApi(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddApplicationPart(typeof(Startup).Assembly);
        }

        public static IIdentityServerBuilder AddIdentityServerAudience(
            this IIdentityServerBuilder builder,
            Action<StartupOptions> options)
        {
            var startupOptions = GetDefaulOptions();
            options(startupOptions);

            const string introspectionScheme = "introspection";
            var authBuilder = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
                {
                    jwtBearerOptions.Authority = startupOptions.Authority;
                    jwtBearerOptions.Audience = startupOptions.Audience;
                    jwtBearerOptions.RequireHttpsMetadata = startupOptions.RequireSsl;

                    if(startupOptions.Introspection == null) return;
                    jwtBearerOptions.ForwardDefaultSelector = Selector.ForwardReferenceToken(introspectionScheme);

                });

            if (startupOptions.Introspection != null)
            {
                authBuilder.AddOAuth2Introspection(introspectionScheme, introspectionOptions =>
                {
                    introspectionOptions.Authority = startupOptions.Authority;
                    introspectionOptions.ClientId = startupOptions.Introspection.Audience;
                    introspectionOptions.ClientSecret = startupOptions.Introspection.Secret;
                    introspectionOptions.DiscoveryPolicy = new DiscoveryPolicy
                    {
                        RequireHttps = startupOptions.RequireSsl
                    };
                });
            }

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
            var identityUserConfig = configuration.GetSection("Identity:User");
            var smtpConfig = configuration.GetSection("Smtp");

            services.Configure<IdentityUserConfig>(identityUserConfig);
            services.Configure<IdentityUserConfig>(identityUserConfig);

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

        private static StartupOptions GetDefaulOptions()
        {
            var options = new StartupOptions();
            return options;
        }
    }

    public class StartupOptions
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public Introspection Introspection { get; set; }
        public bool RequireSsl { get; set; }
    }

    public class Introspection
    {
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}
