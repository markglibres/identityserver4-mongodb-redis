using System.Threading.Tasks;
using EventStore.Client;
using Identity.Infrastructure.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure
{
    public class StreamHandler : IStreamHandler
    {
        private readonly ILogger<StreamHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IEventStoreDbSerializer _eventStoreDbSerializer;

        public StreamHandler(ILogger<StreamHandler> logger,
            IMediator mediator,
            IEventStoreDbSerializer eventStoreDbSerializer)
        {
            _logger = logger;
            _mediator = mediator;
            _eventStoreDbSerializer = eventStoreDbSerializer;
        }
        public async Task Handle(EventRecord @event)
        {
            _logger.LogInformation("StreamHandler at position: {arg2}", @event.Position.CommitPosition);

            var message = await _eventStoreDbSerializer.Deserialize(@event);
            _logger.LogInformation("Message Serialized: ", message);
            
            await _mediator.Publish(message);
            
        }
    }
}