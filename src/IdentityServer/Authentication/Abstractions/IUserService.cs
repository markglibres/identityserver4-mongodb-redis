using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Authentication.Abstractions
{
    public interface IUserService<in T>
    {
        Task Create(T user, CancellationToken cancellationToken = default);
        Task Update(T user, CancellationToken cancellationToken = default);
    }
}