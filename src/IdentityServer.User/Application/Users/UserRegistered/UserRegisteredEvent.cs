using IdentityServer.Management.Application.Abstractions;
using MediatR;

namespace IdentityServer.Management.Application.Users.UserRegistered
{
    public class UserRegisteredEvent : IApplicationEvent, INotification
    {
        public string UserId { get; set; }
    }
}
