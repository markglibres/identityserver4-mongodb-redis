using System;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Abstractions
{
    public interface IReadModel
    {
        string Id { get; set; }
        string LastCommittedEvent { get; set; }
    }
}