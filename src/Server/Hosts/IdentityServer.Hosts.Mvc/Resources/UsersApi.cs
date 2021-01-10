using System.Collections.Generic;
using IdentityServer.Authorization.Seeders;
using IdentityServer4.Models;

namespace IdentityServer.Hosts.Mvc.Resources
{
    public class UsersApiResource : ISeeder<ApiResource>
    {
        public IEnumerable<ApiResource> GetSeeds()
        {
            return new List<ApiResource>
            {
                new ApiResource("UsersApi", "User Management API")
                {
                    Scopes = new List<string>
                    {
                        "users.management"
                    },
                    ApiSecrets = {new Secret("hardtoguess".Sha256())}
                }
            };
        }
    }

    public class UsersApiScopes : ISeeder<ApiScope>
    {
        public IEnumerable<ApiScope> GetSeeds()
        {
            return new[]
            {
                new ApiScope("users.management", "Manage Users Scope")
            };
        }
    }

}
