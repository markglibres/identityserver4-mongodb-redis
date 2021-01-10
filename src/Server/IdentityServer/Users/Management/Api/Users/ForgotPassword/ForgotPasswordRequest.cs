namespace IdentityServer.Users.Management.Api.Users.ForgotPassword
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string ResetUrl { get; set; }
    }
}
