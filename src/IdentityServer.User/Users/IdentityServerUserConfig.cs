namespace IdentityServer.Management.Users
{
    public class IdentityServerUserConfig
    {
        public ConfirmationEmailConfig ConfirmationEmail { get; set; }
        public string BaseUrl { get; set; }
    }

    public class ConfirmationEmailConfig
    {
        public bool Require { get; set; }
        public string Subject { get; set; }
    }
}
