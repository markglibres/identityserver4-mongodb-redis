using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Users.Abstractions
{
    public interface IUserService<T>
    {
        Task Create(T user, CancellationToken cancellationToken = default);
        Task Update(T user, CancellationToken cancellationToken = default);
        Task<T> GetByUsername(string username, CancellationToken cancellationToken = default);
    }
}