using IdentityServer.Management.Application.Users.Urls;
using MediatR;

namespace IdentityServer.Management.Application.Users.RegisterUser
{
    public class RegisterUserCommand : IRequest<RegisterUserCommandResult>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PlainTextPassword { get; set; }
        public ConfirmEmailUrlFormat ConfirmUrl { get; set; }
    }


}
