using MediatR;

namespace IdentityServer.Users.Management.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordCommandResult>
    {
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
        public ResetUrlFormatter ResetUrlFormatter { get; set; }
    }

    public delegate string ResetUrlFormatter(string userId, string token, string returnUrl = null);
}
