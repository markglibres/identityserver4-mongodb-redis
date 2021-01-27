using System.Threading;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Abstractions
{
    public interface IStreamSubscriber
    {
        Task Subscribe(CancellationToken stoppingToken);
    }
}