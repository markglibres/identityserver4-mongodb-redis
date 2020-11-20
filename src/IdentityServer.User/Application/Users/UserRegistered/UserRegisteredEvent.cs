using IdentityServer.Management.Application.Abstractions;
using MediatR;

namespace IdentityServer.Management.Application.Users.Events
{
    public class UserRegisteredEvent : IApplicationEvent, INotification
    {
        public string UserId { get; set; }
    }
}
