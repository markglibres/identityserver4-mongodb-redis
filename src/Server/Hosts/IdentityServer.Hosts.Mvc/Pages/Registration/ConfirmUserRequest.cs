namespace IdentityServer.Hosts.Mvc.ViewModels
{
    public class ConfirmUserRequest
    {
        public string Token { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
