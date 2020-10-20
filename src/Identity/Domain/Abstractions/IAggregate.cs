using System.Collections.Generic;

namespace Identity.Domain.Abstractions
{
    public interface IAggregate<out TId> : IEntityId
    {
        TId Id { get; }
    }

    public interface IAggregate
    {
        IReadOnlyCollection<IDomainEvent> UncommittedEvents { get; }
        IReadOnlyCollection<IDomainEvent> CommittedEvents { get; }
    }

}