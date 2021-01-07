using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace IdentityServer.Management.Registration
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly IOptionsMonitor<OpenIdConnectOptions> _openIdConnectOptions;
        private OpenIdConnectOptions _oidc;

        public UserInteractionService(IOptionsMonitor<OpenIdConnectOptions> openIdConnectOptions)
        {
            _openIdConnectOptions = openIdConnectOptions;
            _oidc = _openIdConnectOptions.Get("oidc");
        }

        public Task<RegistrationContext> GetRegistrationContext()
        {
            return Task.FromResult(new RegistrationContext($"{_oidc.Authority}/identity/registration?cliendId={_oidc.ClientId}"));
        }
    }
}
