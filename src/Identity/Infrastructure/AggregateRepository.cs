using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Abstractions;
using BindingFlags = System.Reflection.BindingFlags;

namespace Identity.Infrastructure
{
    public class AggregateRepository<T, TId> : IAggregateRepository<T, TId> 
        where T: IAggregate<TId>, IAggregate
        where TId: IAggregateId
    {
        private readonly IEventsRepository<IDomainEvent> _eventsRepository;

        public AggregateRepository(IEventsRepository<IDomainEvent> eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }
        
        public async Task Save(T aggregate, CancellationToken cancellationToken)
        {
            var events = aggregate.UncommittedEvents;
            await _eventsRepository.Save(events, GetStreamName(aggregate.Id), cancellationToken);
        }

        public async Task<T> Get(TId id, CancellationToken cancellationToken)
        {
            try
            {
                var events = await _eventsRepository.Get(GetStreamName(id), cancellationToken);

                var constructor = typeof(T).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new [] { id.GetType(), typeof(IReadOnlyCollection<IDomainEvent>) },
                    null
                );
                var aggregate = (T) constructor?.Invoke(new object[] {id, events});

                return aggregate;
            }
            catch (Exception e)
            {
                throw new DomainException($"There was an error loading the aggregate: {e.Message}");
            }
        }

        private string GetStreamName(TId id) => $"{typeof(T).Name}-{id.StreamName}";
    }
}