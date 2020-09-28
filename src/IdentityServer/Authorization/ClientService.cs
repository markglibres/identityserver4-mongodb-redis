using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Repositories;
using IdentityServer4.Models;

namespace IdentityServer.Services
{
    public class ClientService : IClientService, ISeedService<Client>
    {
        private readonly IIdentityRepository<Client> _repository;

        public ClientService(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        public async Task Create(Client client, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return;
            var result = await Get(client.ClientId);
            if (result != null) return;

            await _repository.Insert(client);
        }

        public async Task Update(Client client, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return;
            var result = await Get(client.ClientId);
            if (result == null) return;

            await _repository.Update(client, c => c.ClientId == client.ClientId);
        }

        public async Task Delete(Client client, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return;
            var result = await Get(client.ClientId);
            if (result == null) return;

            await _repository.Delete(c => c.ClientId == client.ClientId);
        }

        public async Task<Client> Get(string clientId, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return default;
            return await _repository.SingleOrDefault(c => c.ClientId == clientId);
        }
    }
}