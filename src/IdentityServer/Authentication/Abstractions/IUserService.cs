using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services
{
    public interface IUserService<in T>
    {
        Task Create(T user, CancellationToken cancellationToken = default);
        Task Update(T user, CancellationToken cancellationToken = default);
    }
}