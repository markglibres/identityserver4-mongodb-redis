using IdentityServer.Management.Application.Abstractions;
using MediatR;

namespace IdentityServer.Management.Application.Users.Notifications.ForgotPasswordRequested
{
    public class ForgotPasswordRequestedEvent : IApplicationEvent, INotification
    {
        public string UserId { get; set; }
        public string Url { get; set; }
    }
}
