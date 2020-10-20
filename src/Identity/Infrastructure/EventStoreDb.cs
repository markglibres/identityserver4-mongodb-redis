using System;
using EventStore.Client;
using Identity.Infrastructure.Abstractions;
using Identity.Infrastructure.Configs;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure
{
    public class EventStoreDb : IEventStoreDb
    {
        private readonly EventStoreClient _eventStoreClient;

        public EventStoreDb(IOptions<EventStoreDbConfig> options)
        {
            var config = options.Value;
            var settings = new EventStoreClientSettings
            {
                ConnectivitySettings =
                {
                    Address = new Uri(config.ConnectionString)
                },
                ConnectionName = config.Name
            };

            _eventStoreClient = new EventStoreClient(settings);
        }

        public EventStoreClient GetClient() => _eventStoreClient;
    }
}