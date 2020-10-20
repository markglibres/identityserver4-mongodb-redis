using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Abstractions
{
    public abstract class Aggregate<TId>: IAggregate, IAggregate<TId>
        where TId: IAggregateId<Guid>
    {
        public TId Id { get; }
        
        private readonly List<IDomainEvent> _unCommittedEvents = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> UncommittedEvents => _unCommittedEvents.ToList().AsReadOnly();
        
        private readonly List<IDomainEvent> _committedEvents = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> CommittedEvents => _committedEvents.ToList().AsReadOnly();

        protected Aggregate(TId id)
        {
            Id = id;
        }
        
        protected Aggregate(TId id, IReadOnlyCollection<IDomainEvent> events) : this(id)
        {
            if(!events.Any()) return;
            events.ToList().ForEach(Apply);
            _committedEvents.AddRange(events);
        }

        protected void Emit(IDomainEvent @event)
        {
            if (_unCommittedEvents.Any(e => e.Id == @event.Id)) return;
            
            Apply(@event);
            _unCommittedEvents.Add(@event);
        }

        private void Apply(IDomainEvent @event)
        {
            var method = GetType()
                .GetMethod(
                    "Handle", 
                    BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.Public, 
                    null,  
                    new[] {@event.GetType()}, 
                    null); 
            
            method?.Invoke(this, new object[] {@event});
        }

    }
}