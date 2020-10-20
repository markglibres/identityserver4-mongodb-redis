using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Infrastructure;
using Identity.Infrastructure.Abstractions;
using Identity.Infrastructure.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity
{
    public static class Startup
    {
        public static IMvcBuilder AddIdentity(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddApplicationPart(typeof(Startup).Assembly);
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var configSection = configuration.GetSection("EventStoreDb");
            services.Configure<EventStoreDbConfig>(configSection);
            
            services.AddSingleton<IEventStoreDb, EventStoreDb>();
            services.AddTransient<IEventStoreDbSerializer, EventStoreDbSerializer>();
            services.AddTransient<IEventsRepository<IDomainEvent>, EventsRepository<IDomainEvent>>();
            
            
            return services;
        }
        
    }
}