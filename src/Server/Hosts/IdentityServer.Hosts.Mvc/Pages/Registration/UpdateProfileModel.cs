namespace IdentityServer.Hosts.Mvc.ViewModels
{
    public class UpdateProfileModel
    {
        public string Token { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ReturnUrl { get; set; }
    }
}
