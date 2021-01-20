using AngleSharp.Text;

namespace IdentityServer.Users.Management.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommandResult
    {
        public string Token { get; set; }
        public string Url { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
