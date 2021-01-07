using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace IdentityServer.Management.Registration
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly UserInteractionServiceOptions _options;
        private readonly IOptionsMonitor<OpenIdConnectOptions> _openIdConnectOptions;
        private OpenIdConnectOptions _oidc;

        public UserInteractionService(UserInteractionServiceOptions options,
            IOptionsMonitor<OpenIdConnectOptions> openIdConnectOptions)
        {
            _options = options;
            _openIdConnectOptions = openIdConnectOptions;
            _oidc = _openIdConnectOptions.Get(_options.AuthenticationScheme);
        }

        public Task<RegistrationContext> GetRegistrationContext()
        {
            return Task.FromResult(new RegistrationContext($"{_oidc.Authority}/identity/registration?cliendId={_oidc.ClientId}"));
        }
    }

    public class UserInteractionServiceOptions
    {
        public string AuthenticationScheme { get; set; }
    }
}
