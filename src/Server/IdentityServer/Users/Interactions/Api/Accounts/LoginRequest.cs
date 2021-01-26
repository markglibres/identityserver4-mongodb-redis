namespace IdentityServer.Users.Interactions.Api.Accounts
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}