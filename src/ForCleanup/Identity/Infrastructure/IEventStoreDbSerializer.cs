using System.Threading.Tasks;
using EventStore.Client;
using Identity.Domain.Abstractions;

namespace Identity.Infrastructure
{
    public interface IEventStoreDbSerializer
    {
        EventData Serialize(IDomainEvent @event);
        Task<IDomainEvent> Deserialize(EventRecord @event);
    }
}