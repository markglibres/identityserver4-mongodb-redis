using System.Security.Principal;
using MediatR;

namespace IdentityServer.Management.Application.Accounts.Logout
{
    public class LogoutCommand : IRequest<LogoutCommandResult>
    {
        public IIdentity Identity { get; set; }
        public string LogoutId { get; set; }
    }
}
