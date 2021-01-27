using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Interactions.Application.Abstractions;
using IdentityServer.Users.Interactions.Application.Users.Events.ForgotPasswordRequested.Templates;
using IdentityServer.Users.Interactions.Infrastructure.Config;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Users.Interactions.Application.Users.Events.ForgotPasswordRequested
{
    public class ForgotPasswordRequestedEventHandler : INotificationHandler<ForgotPasswordRequestedEvent>
    {
        private readonly IEmailer _emailer;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IdentityServerUserInteractionConfig _interactionOptions;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgotPasswordRequestedEventHandler(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityServerUserInteractionConfig> managementOptions,
            IEmailTemplate emailTemplate,
            IEmailer emailer)
        {
            _userManager = userManager;
            _interactionOptions = managementOptions.Value;
            _emailTemplate = emailTemplate;
            _emailer = emailer;
        }

        public async Task Handle(ForgotPasswordRequestedEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(notification.UserId);
            if (user == null) throw new DomainException($"User {notification.UserId} not found.");

            var templateOptions = _interactionOptions.Emails?.ForgotPassword?.TemplateOptions ??
                                  new EmailTemplateOptions
                                  {
                                      File = "user-forgotpassword-request.html",
                                      FileStorageType = FileStorageTypes.Embedded
                                  };
            var subject = _interactionOptions.Emails?.ForgotPassword?.Subject ?? "Reset password";

            var content = await _emailTemplate.Generate(new ForgotPasswordEmailModel
            {
                Url = notification.Url
            }, templateOptions);

            await _emailer.Send(user.Email, subject, content);
        }
    }
}