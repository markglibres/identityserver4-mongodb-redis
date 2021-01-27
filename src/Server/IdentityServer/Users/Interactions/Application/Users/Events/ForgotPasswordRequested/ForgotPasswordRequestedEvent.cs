using IdentityServer.Users.Interactions.Application.Abstractions;
using MediatR;

namespace IdentityServer.Users.Interactions.Application.Users.Events.ForgotPasswordRequested
{
    public class ForgotPasswordRequestedEvent : IApplicationEvent, INotification
    {
        public string UserId { get; set; }
        public string Url { get; set; }
    }
}