using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IdentityModel.Client;
using IdentityServer.Common;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IdentityServer.User.Client.Registration
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUrlBuilder _urlBuilder;
        private readonly UserInteractionServiceOptions _options;
        private readonly IOptionsMonitor<OpenIdConnectOptions> _openIdConnectOptions;
        private readonly HttpClient _httpClient;
        private OpenIdConnectOptions _oidc;

        public UserManagementService(
            IUrlBuilder urlBuilder,
            UserInteractionServiceOptions options,
            IOptionsMonitor<OpenIdConnectOptions> openIdConnectOptions,
            HttpClient httpClient)
        {
            _urlBuilder = urlBuilder;
            _options = options;
            _openIdConnectOptions = openIdConnectOptions;
            _httpClient = httpClient;
            _oidc = _openIdConnectOptions.Get(_options.AuthenticationScheme);
        }

        public async Task<RegistrationContext> GetRegistrationContext(string redirectUrl)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_oidc.Authority);
            if (disco.IsError) throw new Exception(disco.Error);

            var redirectUri = _urlBuilder.Create()
                .Path(redirectUrl)
                .ToString();

            // var authorizeUrl = new RequestUrl(disco.AuthorizeEndpoint).CreateAuthorizeUrl(
            //          clientId: _oidc.ClientId,
            //          responseType: _oidc.ResponseType,
            //          scope: string.Join(" ", _oidc.Scope),
            //          redirectUri: "http://localhost:5002/signin-oidc",
            //          state: "random_state",
            //          nonce: "random_nonce",
            //          responseMode: "form_post");

            // var queryString = HttpUtility.ParseQueryString(new Uri(redirectUrl).Query);
            // var callback = $"{new Uri(disco.TryGetString("authorization_endpoint")).AbsolutePath}";
            // var returnUrl = $"{callback}?{queryString}";
            var registrationUrl = disco.TryGetString("registration_endpoint");

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _oidc?.ClientId,
                ClientSecret = _oidc?.ClientSecret,
                Scope = _options.Scope
            });
            if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

            //var config = await GetConfigurations();

            var url = _urlBuilder
                .Create(registrationUrl)
                .AddQuery("RedirectUrl", redirectUri)
                .AddQuery("token", HttpUtility.UrlEncode(tokenResponse.AccessToken))
                .ToString();

            return new RegistrationContext(url);

            //return new RegistrationContext($"{config.CreateUserPath}?token={HttpUtility.UrlEncode(tokenResponse.AccessToken)}");
        }

        // private async Task<IdentityServerConfig> GetConfigurations()
        // {
        //     var response = await _httpClient.GetAsync($"{_oidc?.Authority}/configurations");
        //     if (!response.IsSuccessStatusCode) return new IdentityServerConfig();
        //
        //     var configString = await response.Content.ReadAsStringAsync();
        //     var config = JsonConvert.DeserializeObject<IdentityServerConfig>(configString);
        //     return config;
        // }
   }

    internal class IdentityServerConfig
    {
        public string CreateUserPath { get; set; }
    }

    public class UserInteractionServiceOptions
    {
        public string AuthenticationScheme { get; set; }
        public string Scope { get; set; }
    }
}
