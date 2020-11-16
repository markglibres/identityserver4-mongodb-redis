using System;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Management
{
    public static class Startup
    {
        public static IMvcBuilder AddIdentityServerUserManagement(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddApplicationPart(typeof(Startup).Assembly);
        }
        
        public static IIdentityServerBuilder AddIdentityServerUserManagement<TUser, TRole>(
            this IIdentityServerBuilder builder)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            return builder.AddIdentityServerUserManagement<TUser, TRole>(options => { });
        }
        
        public static IIdentityServerBuilder AddIdentityServerUserManagement<TUser, TRole>(
            this IIdentityServerBuilder builder,
            Action<StartupOptions> options)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            var startupOptions = GetDefaulOptions();
            options(startupOptions);

            builder.Services.AddMediatR(typeof(Startup).Assembly);
            
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions => { 
                    jwtBearerOptions.Authority = startupOptions.Authority; 
                    jwtBearerOptions.Audience = startupOptions.Audience; 
                    jwtBearerOptions.RequireHttpsMetadata = false; 
                });

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
    }
}