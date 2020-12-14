using System.Collections.Generic;
using IdentityServer.Seeders;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer.Web.Apis
{
    public class IdentityServerClients : ISeeder<Client>
    {
        public IEnumerable<Client> GetSeeds()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "reactWebApp",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("hardtoguess".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    },
                    AllowOfflineAccess = true, // this to allow SPA,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // this will generate reference tokens instead of access tokens
                    AccessTokenType = AccessTokenType.Reference
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = false,
                    AllowOfflineAccess = true,

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    RequirePkce = true

                }
            };
        }
    }
}
