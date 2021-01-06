using MediatR;

namespace IdentityServer.Management.Application.Accounts.Login
{
    public class LoginCommand : IRequest<LoginCommandResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
