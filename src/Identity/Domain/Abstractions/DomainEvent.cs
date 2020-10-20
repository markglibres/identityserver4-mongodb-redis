using System;

namespace Identity.Domain.Abstractions
{
    public abstract class DomainEvent : IDomainEvent
    {
        public string Id { get; private set; }
        public string EntityId { get; private set; }

        protected DomainEvent(IEntityId entityId)
        {
            EntityId = entityId.ToString();
            Id = Guid.NewGuid().ToString();
        }

        public string StreamName => EntityId;
    }
}