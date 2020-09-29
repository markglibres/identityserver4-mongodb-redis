using IdentityServer.Repositories;
using IdentityServer.Services;
using IdentityServer.Stores;
using IdentityServer.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer.Authentication
{
    public static class Startup
    {
        public static IServiceCollection AddIdentityUser<TUser, TRole>(this IServiceCollection services)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            services.AddIdentityMongoDb();
            services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();
            services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
            services.AddTransient<IUserService<TUser>, UserService<TUser>>();

            return services;
        }

        public static IServiceCollection AddSeedUsers<TUser, TSeeder>(this IServiceCollection services)
            where TUser: IdentityUser
            where TSeeder: class, ISeeder<TUser>
        {
            services.TryAddTransient<ISeedService<TUser>, UserService<TUser>>();
            services.AddSingleton<ISeeder<TUser>, TSeeder>();
            return services;
        }
        
        public static IServiceCollection AddSeedUsers<TUser>(this IServiceCollection services)
            where TUser: IdentityUser
        {
            services.AddSeedUsers<TUser, SeedUsers<TUser>>();
            return services;
        }
    }
}