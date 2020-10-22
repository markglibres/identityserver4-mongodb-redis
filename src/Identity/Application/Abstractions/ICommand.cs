using System;

namespace Identity.Application.Abstractions
{
    public interface IAggregateCommand
    {
        string TenantId { get; set; }
        Guid Id { get; set; }
    }
}