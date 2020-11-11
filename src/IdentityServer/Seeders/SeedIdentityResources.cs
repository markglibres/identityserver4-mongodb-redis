using System.Collections.Generic;
using Identity.Common.Seeders;
using IdentityServer4.Models;

namespace IdentityServer.Seeders
{
    public class SeedIdentityResources : ISeeder<IdentityResource>
    {
        public IEnumerable<IdentityResource> GetSeeds()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
    }
}