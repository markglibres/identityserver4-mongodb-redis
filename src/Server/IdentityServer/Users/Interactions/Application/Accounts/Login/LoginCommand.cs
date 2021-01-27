using MediatR;

namespace IdentityServer.Users.Interactions.Application.Accounts.Login
{
    public class LoginCommand : IRequest<LoginCommandResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}