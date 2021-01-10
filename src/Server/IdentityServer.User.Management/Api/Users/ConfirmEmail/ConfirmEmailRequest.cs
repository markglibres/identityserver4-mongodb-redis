namespace IdentityServer.Management.Api.Users.ConfirmEmail
{
    public class ConfirmEmailRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
