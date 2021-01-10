namespace IdentityServer.Users.Interactions.Application.Accounts.Login
{
    public class LoginCommandResult
    {
        public bool IsSuccess { get; set; }
        public string ReturnUrl { get; set; }
    }
}
