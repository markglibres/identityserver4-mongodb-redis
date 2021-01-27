namespace IdentityServer.Hosts.Mvc.ViewModels
{
    public class LoggedoutModel
    {
        public string LogoutId { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }
    }
}