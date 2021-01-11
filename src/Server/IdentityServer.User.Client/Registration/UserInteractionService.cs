using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IdentityServer.User.Client.Registration
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly UserInteractionServiceOptions _options;
        private readonly IOptionsMonitor<OpenIdConnectOptions> _openIdConnectOptions;
        private readonly HttpClient _httpClient;
        private OpenIdConnectOptions _oidc;

        public UserInteractionService(
            UserInteractionServiceOptions options,
            IOptionsMonitor<OpenIdConnectOptions> openIdConnectOptions,
            HttpClient httpClient)
        {
            _options = options;
            _openIdConnectOptions = openIdConnectOptions;
            _httpClient = httpClient;
            _oidc = _openIdConnectOptions.Get(_options.AuthenticationScheme);
        }

        public async Task<RegistrationContext> GetRegistrationContext()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_oidc.Authority);
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _oidc?.ClientId,
                ClientSecret = _oidc?.ClientSecret,
                Scope = _options.Scope
            });
            if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);
            var config = await GetConfigurations();

            return new RegistrationContext($"{config.CreateUserPath}?token={HttpUtility.UrlEncode(tokenResponse.AccessToken)}");
        }

        private async Task<IdentityServerConfig> GetConfigurations()
        {
            var response = await _httpClient.GetAsync($"{_oidc?.Authority}/configurations");
            if (!response.IsSuccessStatusCode) return new IdentityServerConfig();

            var configString = await response.Content.ReadAsStringAsync();
            var config = JsonConvert.DeserializeObject<IdentityServerConfig>(configString);
            return config;
        }
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
