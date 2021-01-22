using MediatR;

namespace IdentityServer.Users.Management.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordCommandResult>
    {
        public string Email { get; set; }
        public string ReturnUrl { get; set; }

    }
}
