using System;
using System.Reflection;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Abstractions
{
    public abstract class AggregateGuid : IAggregateId<Guid>, IAggregateId, IStreamable, IEntityId
    {
        private const string _delimiter = "|";

        protected AggregateGuid()
        {
        }

        protected AggregateGuid(TenantId tenantId, Guid id)
        {
            TenantId = tenantId;
            Id = id;
        }

        protected AggregateGuid(TenantId tenantId)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId;
        }

        protected AggregateGuid(string id)
        {
            var tokens = id.Split(_delimiter);
            TenantId = TenantId.Create(tokens[0]);
            Id = Guid.Parse(tokens[1]);
        }

        public string StreamName => $"{TenantId}-{Id}";
        public TenantId TenantId { get; }
        public Guid Id { get; }

        public override string ToString()
        {
            return $"{TenantId}{_delimiter}{Id}";
        }

        public static TId Create<TId>(string tenantId, Guid id) where TId : class, IAggregateId<Guid>
        {
            var tenant = TenantId.Create(tenantId);
            var constructor = typeof(TId).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] {typeof(TenantId), typeof(Guid)},
                null
            );
            var aggregateId = (TId) constructor?.Invoke(new object[] {tenant, id});

            return aggregateId;
        }
        
        public static TId Create<TId>(string tenantId) where TId : class, IAggregateId<Guid>
        {
            var tenant = TenantId.Create(tenantId);
            var constructor = typeof(TId).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] {typeof(TenantId)},
                null
            );
            var aggregateId = (TId) constructor?.Invoke(new object[] {tenant});

            return aggregateId;
        }
        
        public static TId From<TId>(string aggregateId) where TId : class, IAggregateId<Guid>
        {
            var constructor = typeof(TId).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] {typeof(string)},
                null
            );
            var id = (TId) constructor?.Invoke(new object[] {aggregateId});

            return id;
        }
    }
}