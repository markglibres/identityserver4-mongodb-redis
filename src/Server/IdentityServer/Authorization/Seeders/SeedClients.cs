using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer.Authorization.Seeders
{
    public class SeedClients : ISeeder<Client>
    {
        public IEnumerable<Client> GetSeeds()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "swagger",
                    AllowedGrantTypes = new List<string>
                    {
                        GrantTypes.ClientCredentials.FirstOrDefault(),
                        GrantTypes.ResourceOwnerPassword.First()
                    },
                    ClientSecrets =
                    {
                        new Secret("hardtoguess".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "myapi.access"
                    },
                    AllowOfflineAccess = true,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AccessTokenType = AccessTokenType.Jwt
                },
                new Client
                {

                    ClientId = "spaService",
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
                        "myapi.access"
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "spaWeb",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("hardtoguess".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "myapi.access"
                    },
                    AllowOfflineAccess = true, // this to allow SPA,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // this will generate reference tokens instead of access tokens
                    AccessTokenType = AccessTokenType.Reference
                }
            };
        }
    }
}
