using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Exceptions;
using IdentityServer.Management.Application.Abstractions;
using IdentityServer.Management.Application.Users.Events.ForgotPasswordRequested.Templates;
using IdentityServer.Management.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Management.Application.Users.Events.ForgotPasswordRequested
{
    public class ForgotPasswordRequestedEventHandler : INotificationHandler<ForgotPasswordRequestedEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IEmailer _emailer;

        public ForgotPasswordRequestedEventHandler(
            UserManager<ApplicationUser> userManager,
            IEmailTemplate emailTemplate,
            IEmailer emailer)
        {
            _userManager = userManager;
            _emailTemplate = emailTemplate;
            _emailer = emailer;
        }
        public async Task Handle(ForgotPasswordRequestedEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(notification.UserId);
            if (user == null) throw new DomainException($"User {notification.UserId} not found.");

            var content = await _emailTemplate.Generate(new ForgotPasswordEmailModel
            {
                Url = notification.Url
            }, options => options.File = "user-forgotpassword-request.html");

            await _emailer.Send(user.Email, "Reset password", content);
        }
    }
}
