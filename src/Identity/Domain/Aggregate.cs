using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Identity.Domain
{
    public abstract class Aggregate<TId>: IAggregate, IAggregate<TId, Guid>
        where TId: IAggregateId<Guid>, new()
    {
        public TId Id { get; }
        
        private static readonly Lazy<IList<IDomainEvent>> _lazyUncommitedEvents  = new Lazy<IList<IDomainEvent>>(() => new List<IDomainEvent>());
        private IList<IDomainEvent> _unCommittedEvents = _lazyUncommitedEvents.Value;
        
        public IReadOnlyCollection<IDomainEvent> UncommittedEvents => _unCommittedEvents.ToList().AsReadOnly();
        
        private static readonly Lazy<IList<IDomainEvent>> _lazyCommitedEvents  = new Lazy<IList<IDomainEvent>>(() => new List<IDomainEvent>());
        private IList<IDomainEvent> _committedEvents = _lazyCommitedEvents.Value;
        
        public IReadOnlyCollection<IDomainEvent> CommittedEvents => _committedEvents.ToList().AsReadOnly();

        protected Aggregate()
        {
        }
        
        protected Aggregate(TenantId tenantId) : this(tenantId, Guid.NewGuid())
        {
            
        }

        protected Aggregate(TenantId tenantId, Guid id)
        {
            Id = new TId();
            Id.From(tenantId, id);
        }

        protected Aggregate(IAggregateId<Guid> id)
        {
            Id = (TId) id;
        }

        protected Aggregate(string id)
        {
            Id = new TId();
            Id.From(id);
        }

        protected Aggregate(IReadOnlyCollection<IDomainEvent> events)
        {
            if(!events.Any()) return;
                
            if(events.GroupBy(c => c.EntityId).Count() > 1) 
                throw new DomainException($"Cannot apply events on {GetType().Name} with multiple aggregate ids");
            
            Id = new TId();
            Id.From(events.First().EntityId);
            
            events.ToList().ForEach(Apply);
        }

        protected void Emit(IDomainEvent @event)
        {
            if (_unCommittedEvents.Any(e => e.Id == @event.Id)) return;
            _unCommittedEvents.Add(@event);
            
            Apply(@event);
        }

        protected void Apply(IDomainEvent @event)
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