using System.Collections.Generic;
using IdentityServer.Services;
using IdentityServer4.Models;

namespace IdentityServer.Web
{
    public class SeedApiScopes : ISeeder<ApiScope>
    {
        public IEnumerable<ApiScope> GetSeeds() => new[]
        {
            new ApiScope("myapi.access", "Access API Backend")
        };
    }
}