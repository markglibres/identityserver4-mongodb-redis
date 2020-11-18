using System;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.Client;
using IdentityServer.Management.Common;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Management
{
    public static class Startup
    {
        public static IMvcBuilder AddIdentityServerUserApi(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddApplicationPart(typeof(Startup).Assembly);
        }
        
        public static IIdentityServerBuilder AddIdentityServerUserApi<TUser, TRole>(this IIdentityServerBuilder builder)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            builder.Services.AddMediatR(typeof(Startup).Assembly);
            builder.Services.AddTransient<IMapper, Mapper>();

            return builder;
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