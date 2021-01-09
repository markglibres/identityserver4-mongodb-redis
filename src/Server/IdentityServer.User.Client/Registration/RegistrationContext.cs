using System;
using System.Text;

namespace IdentityServer.User.Client.Registration
{
    public class RegistrationContext
    {
        public RegistrationContext(string clientId, string clientSecret, string registrationUrl)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RegistrationUrl = registrationUrl;
        }
        public string RegistrationUrl { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }

    }
}
