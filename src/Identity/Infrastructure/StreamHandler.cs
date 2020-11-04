using System.Threading.Tasks;
using EventStore.Client;
using Identity.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure
{
    public class StreamHandler : IStreamHandler
    {
        private readonly ILogger<StreamHandler> _logger;

        public StreamHandler(ILogger<StreamHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(EventRecord @event)
        {
            _logger.LogInformation("StreamHandler at position: {arg2}", @event.Position.CommitPosition);
            return Task.CompletedTask;
        }
    }
}