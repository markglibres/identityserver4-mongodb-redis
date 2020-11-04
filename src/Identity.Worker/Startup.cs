using EventStore.Client;
using Identity.Infrastructure;
using Identity.Infrastructure.Abstractions;
using Identity.Infrastructure.Models;
using Identity.Worker.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddHostedService<HostedServices.Worker>();
            
        }
    }
}