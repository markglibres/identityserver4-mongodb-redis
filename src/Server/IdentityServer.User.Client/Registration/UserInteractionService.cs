using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace IdentityServer.User.Client.Registration
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
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_oidc?.ClientId}:{_oidc?.ClientSecret}"));
            return Task.FromResult(new RegistrationContext(
                _oidc?.ClientId,
                _oidc?.ClientSecret,
                $"{_oidc?.Authority}/registration/create?token={HttpUtility.UrlEncode(token)}"));
        }
   }

    public class UserInteractionServiceOptions
    {
        public string AuthenticationScheme { get; set; }
    }
}
