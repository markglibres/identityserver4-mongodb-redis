using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer.Web
{
    public class InMemoryConfig : IIdentityResource, IApiResources, IApiScopes, IClients
    {
        public IEnumerable<ApiResource> GetApiResources()
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

        public IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("myapi.access", "Access API Backend")
            };
        }

        public IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
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
                },
                new Client
                {
                    ClientId = "spaWeb",
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
                    AllowOfflineAccess = true, // this to allow SPA,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // this will generate reference tokens instead of access tokens
                    AccessTokenType = AccessTokenType.Reference
                }
            };
        }

        public IEnumerable<IdentityResource> GetIdentityResources()
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