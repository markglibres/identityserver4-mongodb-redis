using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Abstractions;
using Identity.Infrastructure.Abstractions;

namespace Identity.Infrastructure
{
    public class EventsRepository<T> : IEventsRepository<T>
        where T: IDomainEvent
    {
        private readonly IEventStoreDbSerializer _eventStoreDbSerializer;
        private readonly EventStoreClient _eventStoreClient;

        public EventsRepository(IEventStoreDb eventStoreDb, IEventStoreDbSerializer eventStoreDbSerializer)
        {
            _eventStoreDbSerializer = eventStoreDbSerializer;
            _eventStoreClient = eventStoreDb.GetClient();
        }

        public async Task Save(IReadOnlyCollection<T> events, string streamName, CancellationToken cancellationToken)
        {
            var serializedEvents = events
                .ToList()
                .Select(e => _eventStoreDbSerializer.Serialize(e));

            try
            {
                await _eventStoreClient.AppendToStreamAsync(streamName, StreamState.Any, serializedEvents,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                throw new DomainException($"There was an error saving events to EventStore: {e.Message}");
            }
        }

        public async Task<IReadOnlyCollection<T>> Get(string streamName, CancellationToken cancellationToken)
        {
            var stream = _eventStoreClient.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start,
                cancellationToken: cancellationToken);

            if (await stream.ReadState == ReadState.StreamNotFound) return null;

            var resolvedEvents = await stream
                .Where(e => !e.Event.Data.IsEmpty)
                .ToListAsync(cancellationToken);

            var events = resolvedEvents
                .Select(async @event => await _eventStoreDbSerializer.Deserialize(@event.Event));

            var deserializedEvents = await Task.WhenAll(events);

            return (IReadOnlyCollection<T>) deserializedEvents.ToList().AsReadOnly();
        }
    }
}