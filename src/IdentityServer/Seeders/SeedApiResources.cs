using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer.Seeders
{
    public class SeedApiResources : ISeeder<ApiResource>
    {
        public IEnumerable<ApiResource> GetSeeds()
        {
            return new List<ApiResource>
            {
                new ApiResource("myapi", "My API")
                {
                    Scopes = new List<string>
                    {
                        "myapi.access"
                    },
                    ApiSecrets = {new Secret("hardtoguess".Sha256())}
                }
            };
        }
    }
}