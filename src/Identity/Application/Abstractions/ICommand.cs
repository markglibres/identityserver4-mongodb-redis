using System;

namespace Identity.Application.Abstractions
{
    public interface IAggregateCommand
    {
        Guid Id { get; set; }
    }
}