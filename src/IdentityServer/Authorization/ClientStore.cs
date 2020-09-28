using System.Threading.Tasks;
using IdentityServer.Repositories;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly IIdentityRepository<Client> _repository;

        public ClientStore(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            return await _repository.SingleOrDefault(client => client.ClientId == clientId);
        }
    }
}