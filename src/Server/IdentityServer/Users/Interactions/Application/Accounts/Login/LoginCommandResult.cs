namespace IdentityServer.Users.Interactions.Application.Accounts.Login
{
    public class LoginCommandResult
    {
        public bool IsSuccess { get; set; }
        public string ReturnUrl { get; set; }
        public LoginErrorCode ErrorCode { get; set; }

    }

    public enum LoginErrorCode
    {
        UnconfirmedEmail, InvalidCredentials
    }
}
