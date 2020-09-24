using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer
{
    public interface IUserService<in T> where T: IdentityUser
    {
        Task Create(T user, CancellationToken cancellationToken = default);
        Task Update(T user, CancellationToken cancellationToken = default);
    }
}