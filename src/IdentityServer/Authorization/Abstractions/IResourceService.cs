using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer.Authorization.Abstractions
{
    public interface IResourceService<T> where T : Resource
    
    {
        Task Create(T apiResource, CancellationToken cancellationToken = default);
        Task<T> GetByName(string name);
    }
}