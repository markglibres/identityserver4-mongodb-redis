using System.Threading.Tasks;
using EventStore.Client;

namespace Identity.Infrastructure.Abstractions
{
    public interface IStreamManager
    {
        Task<Position> GetPosition();
        Task SetPosition(Position position);
    }
}