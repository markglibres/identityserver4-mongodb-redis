using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Abstractions;

namespace Identity.Application.Abstractions
{
    public interface IEventsRepository<T>
        where T: IDomainEvent
    {
        Task Save(IReadOnlyCollection<T> events, string streamName, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<T>> Get(string streamName, CancellationToken cancellationToken);
    }
}