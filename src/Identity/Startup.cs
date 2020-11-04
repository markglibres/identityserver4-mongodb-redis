using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Infrastructure;
using Identity.Infrastructure.Abstractions;
using Identity.Infrastructure.Configs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

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
            var eventStoreConfig = configuration.GetSection("EventStoreDb");
            services.Configure<EventStoreDbConfig>(eventStoreConfig);
            
            var mongoDbConfig = configuration.GetSection("MongoDb");
            services.Configure<MongoDbConfig>(mongoDbConfig);
            
            services.AddSingleton<IEventStoreDb, EventStoreDb>();
            services.AddTransient<IEventStoreDbSerializer, EventStoreDbSerializer>();
            services.AddTransient<IEventsRepository<IDomainEvent>, EventsRepository<IDomainEvent>>();
            services.AddTransient(typeof(IAggregateRepository<,>), typeof(AggregateRepository<,>));
            services.AddTransient(typeof(IDocumentRepository<>), typeof(MongoDbRepository<>));
            
            services.AddMediatR(typeof(Startup));
            
            return services;
        }
        
    }
}