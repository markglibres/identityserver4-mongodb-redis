using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Authorization;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Application.Users.Events.UserRegistered.Templates;
using IdentityServer.Users.Management.Configs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Users.Management.Application.Users.Events.UserRegistered
{
    public class SendUserConfirmationEmailHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IEmailTemplate _emailTemplate;
        private readonly IdentityServerUserManagementConfig _managementOptions;
        private readonly IdentityServerConfig _options;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IEmailer _emailer;

        public SendUserConfirmationEmailHandler(
            IOptions<IdentityServerConfig> options,
            IOptions<IdentityServerUserManagementConfig> managementOptions,
            IUserStore<ApplicationUser> userStore,
            UserManager<ApplicationUser> userManager,
            IEmailTemplate emailTemplate,
            IEmailer emailer)
        {
            _managementOptions = managementOptions.Value;
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

            var templateOptions = _managementOptions.Emails?.UserConfirmation?.TemplateOptions ??
                                  new EmailTemplateOptions
                                  {
                                      File = "user-registered-confirmation.html",
                                      FileStorageType = FileStorageTypes.Embedded
                                  };
            var subject = _managementOptions.Emails?.UserConfirmation?.Subject ??
                          "User registration email confirmation";
            var confirmationContent = await _emailTemplate.Generate(new ConfirmationEmailModel
            {
                Url = notification.Url
            }, templateOptions);

            await _emailer.Send(user.Email, subject, confirmationContent);
        }
    }
}
