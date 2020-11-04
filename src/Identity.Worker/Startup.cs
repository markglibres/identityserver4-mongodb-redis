using EventStore.Client;
using Identity.Worker.HostedServices;
using Identity.Worker.Models;
using Identity.Worker.Services;
using Identity.Worker.Services.Abstractions;
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
            services.AddHostedService<HostedServices.Worker>();
            services.AddTransient<IStreamSubscriber, StreamSubscriber>();
            services.AddTransient<IStreamHandler, StreamHandler>();
            
            BsonClassMap.RegisterClassMap<SubscriptionSettings>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }
    }
}