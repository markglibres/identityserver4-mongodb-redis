using System;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Abstractions
{
    public interface IDomainEvent
    {
        public string EventId { get; }
        public string EntityId { get; }
        public DateTimeOffset CreatedOn { get; }
    }
}