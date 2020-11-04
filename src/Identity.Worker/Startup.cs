using Identity.Worker.Models;
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
            
            BsonClassMap.RegisterClassMap<SubscriptionSettings>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }
    }
}