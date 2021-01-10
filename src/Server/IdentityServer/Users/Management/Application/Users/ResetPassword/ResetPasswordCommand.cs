using MediatR;

namespace IdentityServer.Management.Application.Users.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ResetPasswordCommandResult>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
