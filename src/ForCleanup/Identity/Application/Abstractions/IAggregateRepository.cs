using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Abstractions;

namespace Identity.Application.Abstractions
{
    public interface IAggregateRepository<T, in TId> 
        where T: IAggregate<TId>, IAggregate
        where TId: IAggregateId
    {
        Task Save(T aggregate, CancellationToken cancellationToken);
        Task<T> Get(TId id, CancellationToken cancellationToken);
    }
}