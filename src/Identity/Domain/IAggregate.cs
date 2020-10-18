using System.Collections.Generic;

namespace Identity.Domain
{
    public interface IAggregate<out TId, T> where TId: IEntityId<T>
    {
        TId Id { get; }
    }

    public interface IAggregate
    {
        IReadOnlyCollection<IDomainEvent> UncommittedEvents { get; }
        IReadOnlyCollection<IDomainEvent> CommittedEvents { get; }
    }

}