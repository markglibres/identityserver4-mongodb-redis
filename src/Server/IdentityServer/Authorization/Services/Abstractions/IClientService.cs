using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer.Authorization.Services.Abstractions
{
    public interface IClientService
    {
        Task Create(Client client, CancellationToken cancellationToken = default);
        Task Update(Client client, CancellationToken cancellationToken = default);
        Task Delete(Client client, CancellationToken cancellationToken = default);
        Task<Client> Get(string clientId, CancellationToken cancellationToken = default);
    }
}