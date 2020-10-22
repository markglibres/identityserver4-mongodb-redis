using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Identity.Domain.Abstractions
{
    public abstract class DomainEvent : ValueObject, IDomainEvent
    {
        public string Id { get; private set; }
        public string EntityId { get; private set; }

        protected DomainEvent(IEntityId entityId)
        {
            EntityId = entityId.ToString();
            Id = Guid.NewGuid().ToString();
        }
        protected DomainEvent()
        {
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }
    }
}