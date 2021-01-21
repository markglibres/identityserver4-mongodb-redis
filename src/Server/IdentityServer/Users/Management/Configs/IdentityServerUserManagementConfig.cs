using IdentityServer.Users.Management.Application.Abstractions;

namespace IdentityServer.Users.Management.Configs
{
    public class IdentityServerUserManagementConfig
    {
        public IdentityServerUserManagementConfig()
        {
            Routes = new RouteConfig
            {
                CreateUser = "/Registration/CreateUser"
            };
        }
        public Emails Emails { get; set; }
        public string BaseUrl { get; set; }
        public RouteConfig Routes { get; set; }
        public string Scope { get; set; }
    }

    public class ConfirmationEmailConfig
    {
        public bool Require { get; set; }
        public string Subject { get; set; }
    }

    public class RouteConfig
    {
        public string CreateUser { get; set; }
    }

    public class EmailConfig
    {
        public string Subject { get; set; }
        public EmailTemplateOptions TemplateOptions { get; set; }
    }

    public class Emails
    {
        public EmailConfig EmailConfirmation { get; set; }
        public EmailConfig ForgotPassword { get; set; }
    }
}
