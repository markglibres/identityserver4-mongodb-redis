using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public static class Startup
    {
        public static IIdentityServerBuilder AddMongoDbIdentityServer<TUser, TRole>(this IServiceCollection services, 
            Action<IdentityServerOptions> options,
            Func<IServiceProvider, ICorsPolicyService> policy,
            Action<IIdentityServerBuilder> identityBuilder)
            where TUser: IdentityUser
            where TRole: IdentityRole
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            
            services.Configure<MongoOptions>(configuration.GetSection("Identity:Mongo"));
            
            services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();
            services.AddTransient<IUserStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IUserPasswordStore<TUser>, UserStore<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, UserPasswordHasher<TUser>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TRole>>();
            services.AddTransient(typeof(IRepository<>), typeof(MongoRepository<>));
            services.AddTransient<IUserService<TUser>, UserService<TUser>>();

            var builder = services.AddIdentityServer(options);
            identityBuilder(builder);
            builder.AddAspNetIdentity<TUser>();
            services.AddSingleton(policy);

            return builder;
        }
        
        public static async Task SeedUsers<TUser>(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
            where TUser: IdentityUser
        {
            using var scope = serviceProvider.CreateScope();

            var users = scope
                .ServiceProvider
                .GetService<ISeedUsers<TUser>>()
                ?.GetUsers();
            
            if(users == null) return;
            
            var userService = scope
                .ServiceProvider
                .GetService<IUserService<TUser>>();

            foreach (var user in users)
            {
                await userService.Create(user, cancellationToken);
            }
        } 
        
    }
}