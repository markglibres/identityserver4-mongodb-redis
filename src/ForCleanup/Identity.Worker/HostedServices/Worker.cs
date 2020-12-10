using System.Threading;
using System.Threading.Tasks;
using Identity.Infrastructure.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Worker.HostedServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamSubscriber _streamSubscriber;

        public Worker(
            ILogger<Worker> logger, 
            IStreamSubscriber streamSubscriber)
        {
            _logger = logger;
            _streamSubscriber = streamSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.Subscribe(stoppingToken);
        }
    }
}