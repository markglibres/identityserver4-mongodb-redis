using System.Security.Principal;
using MediatR;

namespace IdentityServer.Users.Interactions.Application.Accounts.Logout
{
    public class LogoutCommand : IRequest<LogoutCommandResult>
    {
        public IIdentity Identity { get; set; }
        public string LogoutId { get; set; }
    }
}