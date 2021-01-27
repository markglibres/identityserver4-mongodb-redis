using MediatR;

namespace IdentityServer.Users.Interactions.Application.Users.UpdatePassword
{
    public class UpdatePasswordCommand : IRequest<UpdatePasswordCommandResult>
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
        public string ResetPasswordToken { get; set; }
    }
}