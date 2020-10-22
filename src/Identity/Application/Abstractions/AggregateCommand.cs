using System;
using Identity.Domain.Abstractions;

namespace Identity.Application.Abstractions
{
    public abstract class AggregateCommand<TId> : IAggregateCommand
        where TId : class, IAggregateId<Guid>
    {
        public string TenantId { get; set; }
        public Guid Id { get; set; }

        public TId AggregateId => AggregateGuid.For<TId>(TenantId, Id);
    }
}