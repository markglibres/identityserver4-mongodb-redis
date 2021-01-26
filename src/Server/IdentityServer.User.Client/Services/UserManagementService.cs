using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using IdentityModel.Client;
using IdentityServer.Common;
using IdentityServer.User.Client.Registration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace IdentityServer.User.Client.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly UserInteractionServiceOptions _options;
        private readonly IUrlBuilder _urlBuilder;
        private readonly OpenIdConnectOptions _oidc;

        public UserManagementService(
            IUrlBuilder urlBuilder,
            UserInteractionServiceOptions options,
            IOptionsMonitor<OpenIdConnectOptions> openIdConnectOptions,
            HttpClient httpClient)
        {
            _urlBuilder = urlBuilder;
            _options = options;
            _httpClient = httpClient;
            _oidc = openIdConnectOptions.Get(_options.AuthenticationScheme);
        }

        public async Task<RegistrationContext> GetRegistrationContext(string redirectUrl)
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(_oidc.Authority);
            if (disco.IsError) throw new Exception(disco.Error);

            var redirectUri = _urlBuilder.Create()
                .Path(redirectUrl)
                .ToString();

            var registrationUrl = disco.TryGetString("registration_endpoint");

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _oidc?.ClientId,
                ClientSecret = _oidc?.ClientSecret,
                Scope = _options.Scope
            });
            if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

            var url = _urlBuilder
                .Create(registrationUrl)
                .AddQuery("RedirectUrl", redirectUri)
                .AddQuery("token", HttpUtility.UrlEncode(tokenResponse.AccessToken))
                .ToString();

            return new RegistrationContext(url);
        }
    }

    public class UserInteractionServiceOptions
    {
        public string AuthenticationScheme { get; set; }
        public string Scope { get; set; }
    }
}
