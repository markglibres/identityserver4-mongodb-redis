using System;
using System.Reflection;
using Identity.Domain.ValueObjects;
using Microsoft.Extensions.WebEncoders.Testing;

namespace Identity.Domain.Abstractions
{
    public abstract class AggregateGuid : IAggregateId<Guid>, IAggregateId
    {
        private const string _delimiter = "|";

        protected AggregateGuid()
        {
        }

        protected AggregateGuid(Guid id, TenantId tenantId = null)
        {
            TenantId = tenantId ?? TenantId.Default;
            Id = id;
        }

        protected AggregateGuid(TenantId tenantId = null)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId ?? TenantId.Default;
        }

        protected AggregateGuid(string id)
        {
            var tokens = id.Split(_delimiter);
            TenantId = TenantId.Create(tokens[0]);
            Id = Guid.Parse(tokens[1]);
        }

        public string StreamName => $"{TenantId}-{GetType().Name}:{Id}";
        public TenantId TenantId { get; }
        public Guid Id { get; }

        public override string ToString()
        {
            return $"{TenantId}{_delimiter}{Id}";
        }

        protected static TId Create<TId>(Guid id, string tenantId = null) where TId : class, IAggregateId<Guid>
        {
            var tenant = string.IsNullOrWhiteSpace(tenantId) ? TenantId.Default : TenantId.Create(tenantId);
            var constructor = typeof(TId).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] {typeof(Guid), typeof(TenantId)},
                null
            );
            var aggregateId = (TId) constructor?.Invoke(new object[] {id, tenant});

            return aggregateId;
        }
        
        protected static TId Create<TId>(string tenantId = null) where TId : class, IAggregateId<Guid>
        {
            var tenant = string.IsNullOrWhiteSpace(tenantId) ? TenantId.Default : TenantId.Create(tenantId);
            var constructor = typeof(TId).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] {typeof(TenantId)},
                null
            );
            var aggregateId = (TId) constructor?.Invoke(new object[] {tenant});

            return aggregateId;
        }

        protected static TId From<TId>(string aggregateId) where TId : class, IAggregateId<Guid>
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