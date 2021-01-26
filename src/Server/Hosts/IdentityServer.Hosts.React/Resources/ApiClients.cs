using System.Collections.Generic;
using IdentityServer.Authorization.Seeders;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer.Hosts.React.Resources
{
    public class ApiClients : ISeeder<Client>
    {
        public IEnumerable<Client> GetSeeds()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "UserManagementApp",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("hardtoguess".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "users.management"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}