using System;
using Identity.Common.Repositories;
using Identity.Common.Repositories.Abstractions;
using Identity.Common.Seeders;
using Identity.Common.Users;
using Identity.Common.Users.Abstractions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Identity.Common
{
    public static class Startup
    {
        public static IServiceCollection AddIdentityMongoDb(
            this IServiceCollection services)
        {
            var config = services.AddIdentityMongoConfig();
            services.TryAddTransient(typeof(IIdentityRepository<>), typeof(IdentityMongoRepository<>));
            
            return services;
        }

        public static IServiceCollection AddIdentityUser<TUser, TRole>(
            this IServiceCollection services)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();
            services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
            services.AddTransient<IUserService<TUser>, UserService<TUser>>();

            return services;
        }
        
        public static IServiceCollection AddIdentityUserSeeder<TUser, TSeeder>(this IServiceCollection services)
            where TUser: IdentityUser
            where TSeeder: class, ISeeder<TUser>
        {
            services.TryAddTransient<ISeedService<TUser>, UserService<TUser>>();
            services.AddSingleton<ISeeder<TUser>, TSeeder>();
            return services;
        }
        
        private static IdentityMongoOptions AddIdentityMongoConfig(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();

            var config = provider.GetService<IOptions<IdentityMongoOptions>>();
            
            if (!string.IsNullOrWhiteSpace(config?.Value?.ConnectionString)) return config.Value;
            
            var mongoSection = configuration.GetSection("Identity:Mongo");
            
            services.Configure<IdentityMongoOptions>(mongoSection);

            var mongoConfig = mongoSection.Get<IdentityMongoOptions>();

            return mongoConfig;
        }
    }
}