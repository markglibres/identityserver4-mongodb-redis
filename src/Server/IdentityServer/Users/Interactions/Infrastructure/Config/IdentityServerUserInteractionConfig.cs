using IdentityServer.Users.Interactions.Application.Abstractions;

namespace IdentityServer.Users.Interactions.Infrastructure.Config
{
    public class IdentityServerUserInteractionConfig
    {
        public IdentityServerUserInteractionConfig()
        {
            UserInteractionEndpoints = new UserInteractionEndpoints
            {
                CreateUser = "/Registration/CreateUser",
                ConfirmUser = "/Registration/Confirm",
                ResetPassword = "/Registration/ResetPassword",
                LoginUrl = "/Account/Login",
                LogoutUrl = "/Account/Logout"
            };
        }

        public Emails Emails { get; set; }
        public string BaseUrl { get; set; }
        public UserInteractionEndpoints UserInteractionEndpoints { get; set; }
        public string Scope { get; set; }
    }

    public class ConfirmationEmailConfig
    {
        public bool Require { get; set; }
        public string Subject { get; set; }
    }

    public class UserInteractionEndpoints
    {
        public string CreateUser { get; set; }
        public string ConfirmUser { get; set; }
        public string ResetPassword { get; set; }
        public string BaseUrl { get; set; }
        public string LoginUrl { get; set; }
        public string LogoutUrl { get; set; }
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
