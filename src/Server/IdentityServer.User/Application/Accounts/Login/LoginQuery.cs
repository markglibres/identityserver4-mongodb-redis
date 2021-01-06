using MediatR;

namespace IdentityServer.Management.Application.Accounts.Login
{
    public class LoginQuery : IRequest<LoginQueryResult>
    {
        public string ReturnUrl { get; set; }
    }
}
