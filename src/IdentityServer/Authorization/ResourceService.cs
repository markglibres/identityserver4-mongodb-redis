using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Repositories;
using IdentityServer4.Models;

namespace IdentityServer.Services
{
    public class ResourceService<T> : IResourceService<T>, ISeedService<T>
        where T: Resource
    {
        private readonly IIdentityRepository<T> _repository;

        public ResourceService(IIdentityRepository<T> repository)
        {
            _repository = repository;
        }
        public async Task Create(T apiResource, CancellationToken cancellationToken = default)
        {
            if(cancellationToken.IsCancellationRequested) return;

            var result = await GetByName(apiResource.Name);
            if (result != null) return;

            await _repository.Insert(apiResource);
        }

        public async Task<T> GetByName(string name)
        {
            return await _repository.SingleOrDefault(r => r.Name == name);
        }
    }
}