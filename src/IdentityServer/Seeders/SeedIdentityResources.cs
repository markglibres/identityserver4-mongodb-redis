using System.Collections.Generic;
using IdentityServer.Services;
using IdentityServer4.Models;

namespace IdentityServer.Web
{
    public class SeedIdentityResources : ISeeder<IdentityResource>
    {
        public IEnumerable<IdentityResource> GetSeeds() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };
    }
}