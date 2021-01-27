using System;
using Identity.Domain.Abstractions;

namespace Identity.Application.Abstractions
{
    public abstract class AggregateCommand<TId> : IAggregateCommand
        where TId : class, IAggregateId<Guid>
    {
        public Guid Id { get; set; }
    }
}