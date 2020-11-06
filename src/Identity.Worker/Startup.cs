using EventStore.Client;
using Identity.Infrastructure;
using Identity.Infrastructure.Abstractions;
using Identity.Infrastructure.Models;
using Identity.Worker.HostedServices;
using IdentityServer.Extensions;
using IdentityServer.Repositories;
using IdentityServer.Repositories.Abstractions;
using IdentityServer.Seeders;
using IdentityServer.Users;
using IdentityServer.Users.Abstractions;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;

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

            services.AddIdentityServerMongoDb(provider =>
                    new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
                    {
                        AllowAll = true
                    })
                .AddRedisCache()
                .AddDeveloperSigningCredential()
                .AddResourceOwnerPassword<ApplicationUser, ApplicationRole>();
            
            services.AddHostedService<HostedServices.Worker>();

            services.AddMediatR(typeof(Startup));

        }
    }
}