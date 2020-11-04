using System;

namespace Identity.Domain.Abstractions
{
    public interface IStreamable
    {
        string StreamName { get; }
    }
}