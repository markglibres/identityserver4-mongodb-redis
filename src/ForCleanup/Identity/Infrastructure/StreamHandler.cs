using System.Threading.Tasks;
using EventStore.Client;
using Identity.Infrastructure.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure
{
    public class StreamHandler : IStreamHandler
    {
        private readonly ILogger<StreamHandler> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEventStoreDbSerializer _eventStoreDbSerializer;

        public StreamHandler(ILogger<StreamHandler> logger,
            IServiceScopeFactory serviceScopeFactory,
            IEventStoreDbSerializer eventStoreDbSerializer)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _eventStoreDbSerializer = eventStoreDbSerializer;
        }
        public async Task Handle(EventRecord @event)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            _logger.LogInformation("StreamHandler at position: {arg2}", @event.Position.CommitPosition);

            var message = await _eventStoreDbSerializer.Deserialize(@event);
            
            await mediator.Publish(message);
            
        }
    }
}