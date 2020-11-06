using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;

namespace Identity.Domain.Abstractions
{
    public abstract class DomainEvent : ValueObject, IDomainEvent, INotification
    {
        public string EventId { get; private set; }
        public string EntityId { get; private set; }
        public string TenantId { get; private set; }
        public DateTimeOffset CreatedOn { get; private set; }

        protected DomainEvent(string entityId, string tenantId = null)
        {
            EntityId = entityId;
            TenantId = string.IsNullOrWhiteSpace(tenantId) ? ValueObjects.TenantId.Default.ToString() : tenantId ;
            EventId = Guid.NewGuid().ToString();
        }
        protected DomainEvent()
        {
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EventId;
        }
    }
}