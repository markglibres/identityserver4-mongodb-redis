using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Authorization;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Interactions.Application.Abstractions;
using IdentityServer.Users.Interactions.Application.Users.Events.UserRegistered.Templates;
using IdentityServer.Users.Interactions.Infrastructure.Config;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Users.Interactions.Application.Users.Events.UserRegistered
{
    public class SendUserConfirmationEmailHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IEmailer _emailer;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IdentityServerUserInteractionConfig _interactionOptions;
        private readonly IdentityServerConfig _options;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public SendUserConfirmationEmailHandler(
            IOptions<IdentityServerConfig> options,
            IOptions<IdentityServerUserInteractionConfig> managementOptions,
            IUserStore<ApplicationUser> userStore,
            UserManager<ApplicationUser> userManager,
            IEmailTemplate emailTemplate,
            IEmailer emailer)
        {
            _interactionOptions = managementOptions.Value;
            _options = options.Value;
            _userStore = userStore;
            _userManager = userManager;
            _emailTemplate = emailTemplate;
            _emailer = emailer;
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            if (!_options.RequireConfirmedEmail || string.IsNullOrWhiteSpace(notification.Url)) return;

            var user = await _userStore.FindByIdAsync(notification.UserId, cancellationToken);

            var templateOptions = _interactionOptions.Emails?.EmailConfirmation?.TemplateOptions ??
                                  new EmailTemplateOptions
                                  {
                                      File = "user-registered-confirmation.html",
                                      FileStorageType = FileStorageTypes.Embedded
                                  };
            var subject = _interactionOptions.Emails?.EmailConfirmation?.Subject ??
                          "User registration email confirmation";
            var confirmationContent = await _emailTemplate.Generate(new ConfirmationEmailModel
            {
                Url = notification.Url
            }, templateOptions);

            await _emailer.Send(user.Email, subject, confirmationContent);
        }
    }
}