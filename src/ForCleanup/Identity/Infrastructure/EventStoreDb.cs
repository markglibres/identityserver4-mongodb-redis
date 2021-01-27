using System;
using System.Net.Http;
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
                DefaultCredentials = new UserCredentials("admin", "changeit"),
                ConnectivitySettings =
                {
                    Address = new Uri(config.ConnectionString)
                },
                ConnectionName = config.Name,
                CreateHttpMessageHandler = () =>
                    new HttpClientHandler {
                        ServerCertificateCustomValidationCallback =
                            (message, certificate2, x509Chain, sslPolicyErrors) => true
                    }
            };

            _eventStoreClient = new EventStoreClient(settings);
        }

        public EventStoreClient GetClient() => _eventStoreClient;
    }
}