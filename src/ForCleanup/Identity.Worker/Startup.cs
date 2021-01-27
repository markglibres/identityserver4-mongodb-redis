using IdentityServer.Extensions;
using IdentityServer.Management;
using IdentityServer.Management.Users;
using IdentityServer4.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Worker
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity(_configuration);
            services.AddIdentitySubscriber();

            services.AddIdentityServerMongoDb()
                .AddRedisCache()
                .AddDeveloperSigningCredential()
                .AddIdentityServerUser<ApplicationUser, ApplicationRole>();

            services.AddHostedService<HostedServices.Worker>();

            services.AddMediatR(typeof(Startup));
        }
    }
}
