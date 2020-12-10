using Identity.Domain.ValueObjects;

namespace Identity.Domain.Abstractions
{
    public interface IAggregateId<T> : IEntityId<T>
    {
        TenantId TenantId { get; }
        T Id { get; }
    }

    public interface IAggregateId : IStreamable
    {
        
    }
}