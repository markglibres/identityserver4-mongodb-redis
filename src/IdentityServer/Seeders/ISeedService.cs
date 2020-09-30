using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Seeders
{
    public interface ISeedService<in T> where T : class
    {
        Task Create(T item, CancellationToken cancellationToken = default);
    }
}