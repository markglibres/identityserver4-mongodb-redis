using IdentityServer.Users.Interactions.Application.Abstractions;
using MediatR;

namespace IdentityServer.Users.Interactions.Application.Users.Events.UserRegistered
{
    public class UserRegisteredEvent : IApplicationEvent, INotification
    {
        public string UserId { get; set; }
        public string Url { get; set; }
    }
}