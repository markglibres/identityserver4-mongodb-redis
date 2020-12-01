using IdentityServer.Management.Application.Users.Urls;
using MediatR;

namespace IdentityServer.Management.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordCommandResult>
    {
        public string Email { get; set; }
        public ResetPasswordUrlFormat ResetPasswordUrl { get; set; }
    }
}
