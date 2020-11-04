using System.Threading.Tasks;
using EventStore.Client;

namespace Identity.Infrastructure.Abstractions
{
    public interface IStreamHandler
    {
        Task Handle(EventRecord arg2Event);
    }
}