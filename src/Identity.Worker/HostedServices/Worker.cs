using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Identity.Infrastructure;
using Identity.Infrastructure.Abstractions;
using Identity.Worker.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Worker.HostedServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEventStoreDbSerializer _eventStoreDbSerializer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private EventStoreClient _eventStoreClient;

        public Worker(
            ILogger<Worker> logger, 
            IEventStoreDb eventStoreDb, 
            IEventStoreDbSerializer eventStoreDbSerializer,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _eventStoreDbSerializer = eventStoreDbSerializer;
            _serviceScopeFactory = serviceScopeFactory;
            _eventStoreClient = eventStoreDb.GetClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var subscriptionRepo =
                scope.ServiceProvider.GetRequiredService<IDocumentRepository<SubscriptionSettings>>();
                
            var settings = await subscriptionRepo.SingleOrDefault("global",
                 s => s.Tenant == "dev" );

            if (settings == null)
            {
                settings = SubscriptionSettings.For("dev");
                await subscriptionRepo.Insert("global", settings);
            }

            //while (!stoppingToken.IsCancellationRequested)
            //{
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await _eventStoreClient.SubscribeToAllAsync(
                    settings.Position,
                    EventAppeared,
                    filterOptions: new SubscriptionFilterOptions(
                        EventTypeFilter.RegularExpression("^User"),
                        checkpointReached: CheckpointReached
                        ),
                    cancellationToken: stoppingToken
                    );
                await Task.Delay(1000, stoppingToken);
            //}
        }

        private async Task CheckpointReached(StreamSubscription arg1, Position arg2, CancellationToken arg3)
        {
            //_logger.LogInformation("CheckpointReached at position: {arg2}", arg2.CommitPosition);
        }

        private async Task EventAppeared(StreamSubscription arg1, ResolvedEvent arg2, CancellationToken arg3)
        {
            var message = await _eventStoreDbSerializer.Deserialize(arg2.Event);
            
            _logger.LogInformation("EventAppeared at position: {arg2}", arg2.OriginalPosition?.CommitPosition);
            
            _logger.LogInformation("Message Id: {id}, Created on: {date}, Entity id: {entityId}", message.Id, message.CreatedOn, message.EntityId);
            
            using var scope = _serviceScopeFactory.CreateScope();

            var subscriptionRepo =
                scope.ServiceProvider.GetRequiredService<IDocumentRepository<SubscriptionSettings>>();
                
            var settings = await subscriptionRepo.SingleOrDefault("global",
                               s => s.Tenant == "dev");
            
            settings.SetLastPosition((long)arg2.Event.Position.CommitPosition);
            await subscriptionRepo.Update("global", settings, s => s.Id.Equals(settings.Id));
        }
    }
}