using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer
{
    public interface IClientService
    {
        Task Create(Client client);
        Task Update(Client client);
        Task Delete(Client client);
        Task<Client> Get(string clientId);
    }
}