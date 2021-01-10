using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            var config = await GetConfigurations();
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_oidc?.ClientId}:{_oidc?.ClientSecret}"));
            return new RegistrationContext($"{config.CreateUserPath}?token={HttpUtility.UrlEncode(token)}");
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
    }
}
