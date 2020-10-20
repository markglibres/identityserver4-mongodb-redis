using System;
using Identity.Domain.User;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Abstractions
{
    public abstract class AggregateGuidId : IAggregateId<Guid>, IAggregateId, IStreamable, IEntityId
    {
        private const string _delimiter = "|";
        public TenantId TenantId { get; private set; }
        public Guid Id { get; private set; }

        protected AggregateGuidId()
        {
            
        }
        
        protected AggregateGuidId(TenantId tenantId, Guid id)
        {
            TenantId = tenantId;
            Id = id;
        }
        
        protected AggregateGuidId(TenantId tenantId)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId;
        }
        
        protected AggregateGuidId(string id)
        {
            var tokens = id.Split(_delimiter);
            TenantId = new TenantId(tokens[0]);
            Id = Guid.Parse(tokens[1]);
        }

        public override string ToString() => $"{TenantId}{_delimiter}{Id}";
        public string StreamName => $"Tenant:{TenantId}-{GetType().Namespace}:{Id}";
    }
}