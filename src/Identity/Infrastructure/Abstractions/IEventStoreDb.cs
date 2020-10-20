using EventStore.Client;

namespace Identity.Infrastructure.Abstractions
{
    public interface IEventStoreDb
    {
        EventStoreClient GetClient();
    }
}