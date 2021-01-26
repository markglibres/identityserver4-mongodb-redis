using MediatR;

namespace IdentityServer.Users.Interactions.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordCommandResult>
    {
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
    }
}