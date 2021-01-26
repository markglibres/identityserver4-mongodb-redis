namespace IdentityServer.Users.Interactions.Api.Users.ConfirmEmail
{
    public class ConfirmEmailRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string ReturnUrl { get; set; }
    }
}