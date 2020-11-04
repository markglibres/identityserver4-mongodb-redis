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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IStreamSubscriber _streamSubscriber;

        public Worker(
            ILogger<Worker> logger,
            IServiceScopeFactory serviceScopeFactory, IStreamSubscriber streamSubscriber)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _streamSubscriber = streamSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var streamSubscriber = scope.ServiceProvider.GetRequiredService<IStreamSubscriber>();

            await _streamSubscriber.Subscribe(stoppingToken);
        }
    }
}