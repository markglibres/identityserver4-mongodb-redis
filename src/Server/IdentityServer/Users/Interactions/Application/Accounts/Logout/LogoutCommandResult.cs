namespace IdentityServer.Management.Application.Accounts.Logout
{
    public class LogoutCommandResult
    {
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }
        public string LogoutId { get; set; }
    }
}
