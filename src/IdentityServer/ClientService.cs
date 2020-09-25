using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _repository;

        public ClientService(IRepository<Client> repository)
        {
            _repository = repository;
        }
        
        public async Task Create(Client client)
        {
            var result = await Get(client.ClientId);
            if(result != null) return;

            await _repository.Insert(client);
        }

        public async Task Update(Client client)
        {
            var result = await Get(client.ClientId);
            if(result == null) return;

            await _repository.Update(client, c => c.ClientId == client.ClientId);
        }

        public async Task Delete(Client client)
        {
            var result = await Get(client.ClientId);
            if(result == null) return;

            await _repository.Delete(c => c.ClientId == client.ClientId);
        }

        public async Task<Client> Get(string clientId)
        {
            return await _repository.SingleOrDefault(c => c.ClientId == clientId);
        }
    }
}