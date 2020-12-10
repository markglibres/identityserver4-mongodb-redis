using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer.Seeders
{
    public class SeedApiScopes : ISeeder<ApiScope>
    {
        public IEnumerable<ApiScope> GetSeeds()
        {
            return new[]
            {
                new ApiScope("myapi.access", "Access API Backend"),
            };
        }
    }
}
