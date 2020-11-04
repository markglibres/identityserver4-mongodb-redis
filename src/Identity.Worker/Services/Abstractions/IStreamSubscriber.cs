using System.Threading.Tasks;
using EventStore.Client;

namespace Identity.Worker.Services.Abstractions
{
    public interface IStreamSubscriber
    {
        Task<Position> GetPosition();
        Task SetPosition(Position position);
    }
}