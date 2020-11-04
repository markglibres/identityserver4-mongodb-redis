using System.Threading.Tasks;
using EventStore.Client;

namespace Identity.Worker.Services.Abstractions
{
    internal interface IStreamHandler
    {
        Task Handle(EventRecord arg2Event);
    }
}