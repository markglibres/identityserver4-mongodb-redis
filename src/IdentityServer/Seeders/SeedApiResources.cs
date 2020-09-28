using System.Collections.Generic;
using IdentityServer.Services;
using IdentityServer4.Models;

namespace IdentityServer.Web
{
    public class SeedApiResources : ISeeder<ApiResource>
    {
        public IEnumerable<ApiResource> GetSeeds() => new List<ApiResource>
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