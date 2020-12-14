using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Management.Users.Abstractions
{
    public interface IUserService<T>
    {
        Task Create(T user, CancellationToken cancellationToken = default);
        Task Update(T user, CancellationToken cancellationToken = default);
        Task<T> GetByUsername(string username, CancellationToken cancellationToken = default);
        Task<bool> ValidateCredentials(string username, string password);
    }
}
