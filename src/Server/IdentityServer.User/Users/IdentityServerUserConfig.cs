using Humanizer;

namespace IdentityServer.Management.Users
{
    public class IdentityServerUserConfig
    {
        public IdentityServerUserConfig()
        {
            Interaction = new Interaction
            {
                CreateUserPath = "/Registration/Create"
            };
        }
        public ConfirmationEmailConfig ConfirmationEmail { get; set; }
        public string BaseUrl { get; set; }
        public Interaction Interaction { get; set; }
    }

    public class ConfirmationEmailConfig
    {
        public bool Require { get; set; }
        public string Subject { get; set; }
    }

    public class Interaction
    {
        public string CreateUserPath { get; set; }
    }
}
