using MediatR;

namespace IdentityServer.Management.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordCommandResult>
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }
}
