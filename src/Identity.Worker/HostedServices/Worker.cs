using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Identity.Infrastructure;
using Identity.Infrastructure.Abstractions;
using Identity.Worker.Models;
using Identity.Worker.Services.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Worker.HostedServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(
            ILogger<Worker> logger, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var eventStoreDb = scope.ServiceProvider.GetRequiredService<IEventStoreDb>();
            var subscriber = scope.ServiceProvider.GetRequiredService<IStreamSubscriber>();
            var streamHandler = scope.ServiceProvider.GetRequiredService<IStreamHandler>();

            var eventStoreClient = eventStoreDb.GetClient();
            
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await eventStoreClient.SubscribeToAllAsync(
                await subscriber.GetPosition(),
                async (subscription, @event, cancellationToken) =>
                {
                    _logger.LogInformation("EventAppeared at position: {arg2}", @event.OriginalPosition?.CommitPosition);
                    await streamHandler.Handle(@event.Event);
                    await subscriber.SetPosition(@event.Event.Position);
                },
                cancellationToken: stoppingToken
                );
            
        }
    }   
}