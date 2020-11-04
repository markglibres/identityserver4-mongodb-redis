using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Identity.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure
{
    public class StreamSubscriber : IStreamSubscriber
    {
        private readonly ILogger<StreamSubscriber> _logger;
        private readonly IStreamManager _streamManager;
        private readonly IStreamHandler _streamHandler;
        private EventStoreClient _eventStoreClient;

        public StreamSubscriber(ILogger<StreamSubscriber> logger,
            IEventStoreDb eventStoreDb,
            IStreamManager streamManager,
            IStreamHandler streamHandler)
        {
            _logger = logger;
            _streamManager = streamManager;
            _streamHandler = streamHandler;

            _eventStoreClient = eventStoreDb.GetClient();
        }
        
        public async Task Subscribe(CancellationToken stoppingToken)
        {
            await _eventStoreClient.SubscribeToAllAsync(
                await _streamManager.GetPosition(),
                async (subscription, @event, cancellationToken) =>
                {
                    _logger.LogInformation("EventAppeared at position: {arg2}", @event.OriginalPosition?.CommitPosition);
                    await _streamHandler.Handle(@event.Event);
                    await _streamManager.SetPosition(@event.Event.Position);
                },
                cancellationToken: stoppingToken
            );
        }
    }
}