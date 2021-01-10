using MediatR;

namespace IdentityServer.Users.Interactions.Application.Accounts.Login
{
    public class LoginQuery : IRequest<LoginQueryResult>
    {
        public string ReturnUrl { get; set; }
    }
}
