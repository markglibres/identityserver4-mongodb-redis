using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Repositories;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Stores
{
    public class ResourceStore : IResourceStore
    {
        private readonly IRepository<ApiResource> _apiResourceRepository;
        private readonly IRepository<ApiScope> _apiScopeRepository;
        private readonly IRepository<IdentityResource> _identityResourceRepository;

        public ResourceStore(
            IRepository<IdentityResource> identityResourceRepository,
            IRepository<ApiScope> apiScopeRepository,
            IRepository<ApiResource> apiResourceRepository)
        {
            _identityResourceRepository = identityResourceRepository;
            _apiScopeRepository = apiScopeRepository;
            _apiResourceRepository = apiResourceRepository;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(
            IEnumerable<string> scopeNames)
        {
            var scopesToSearch = scopeNames.ToList();
            if (!scopesToSearch.Any()) return default;

            var result = await _identityResourceRepository
                .Find(r => scopesToSearch.Contains(r.Name));

            return result;
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var scopesToSearch = scopeNames.ToList();
            if (!scopesToSearch.Any()) return default;

            var result = await _apiScopeRepository
                .Find(r => scopesToSearch.Contains(r.Name));

            return result;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var scopesToSearch = scopeNames.ToList();
            if (!scopesToSearch.Any()) return default;

            var result = await _apiResourceRepository
                .FindAnyIn(nameof(ApiResource.Scopes), scopesToSearch);

            return result;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var namesToSearch = apiResourceNames.ToList();
            if (!namesToSearch.Any()) return default;

            var result = await _apiResourceRepository
                .Find(r => namesToSearch.Contains(r.Name));

            return result;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var identityResources = await _identityResourceRepository.Find(r => true);
            var apiScopes = await _apiScopeRepository.Find(r => true);
            var apiResources = await _apiResourceRepository.Find(r => true);

            return new Resources(identityResources, apiResources, apiScopes);
        }
    }
}