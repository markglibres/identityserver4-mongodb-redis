using System;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Abstractions
{
    public abstract class AggregateGuidId : IAggregateId<Guid>, IAggregateId, IStreamable
    {
        private const string _delimiter = "|";
        public TenantId TenantId { get; private set; }
        public Guid Id { get; private set; }
        
        public AggregateGuidId()
        {
            
        }
        
        public void From(string id)
        {
            var tokens = id.Split(_delimiter);
            TenantId = new TenantId(tokens[0]);
            Id = Guid.Parse(tokens[1]);
        }

        public void From(TenantId tenantId)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId;
        }

        public void From(TenantId tenantId, Guid id)
        {
            TenantId = tenantId;
            Id = id;
        }
      
        public override string ToString() => $"{TenantId}{_delimiter}{Id}";
        public string StreamName => $"Tenant:{TenantId}-{GetType().Namespace}:{Id}";
    }
}