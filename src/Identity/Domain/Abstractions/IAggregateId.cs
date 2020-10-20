using Identity.Domain.ValueObjects;

namespace Identity.Domain.Abstractions
{
    public interface IAggregateId<T> : IEntityId<T>
    {
        TenantId TenantId { get; }
        T Id { get; }
        void From(string id);
        void From(TenantId tenantId);
        void From(TenantId tenantId, T id);
    }

    public interface IAggregateId : IStreamable
    {
        
    }
}