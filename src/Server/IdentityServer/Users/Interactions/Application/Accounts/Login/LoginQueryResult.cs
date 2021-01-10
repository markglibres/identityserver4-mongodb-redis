namespace IdentityServer.Management.Application.Accounts.Login
{
    public class LoginQueryResult
    {
        public bool IsValid { get; set; }
        public string ReturnUrl { get; set; }
        public string Username { get; set; }
    }
}
