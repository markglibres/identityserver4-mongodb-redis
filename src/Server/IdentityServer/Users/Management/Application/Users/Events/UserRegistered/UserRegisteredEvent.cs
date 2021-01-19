using IdentityServer.Users.Management.Application.Abstractions;
using MediatR;

namespace IdentityServer.Users.Management.Application.Users.Events.UserRegistered
{
    public class UserRegisteredEvent : IApplicationEvent, INotification
    {
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string Url { get; set; }

    }
}
