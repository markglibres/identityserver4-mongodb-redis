using System.Threading;
using System.Threading.Tasks;
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
        private readonly IdentityServerUserManagementConfig _options;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IUserStore<ApplicationUser> _userStore;
        private IEmailer _emailer;

        public SendUserConfirmationEmailHandler(
            IOptions<IdentityServerUserManagementConfig> options,
            IUserStore<ApplicationUser> userStore,
            UserManager<ApplicationUser> userManager,
            IEmailTemplate emailTemplate,
            IEmailer emailer)
        {
            _options = options.Value;
            _userStore = userStore;
            _userManager = userManager;
            _emailTemplate = emailTemplate;
            _emailer = emailer;
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            if (!_options.ConfirmationEmail.Require) return;

            var user = await _userStore.FindByIdAsync(notification.UserId, cancellationToken);

            var confirmationContent = await _emailTemplate.Generate(new ConfirmationEmailModel
            {
                Url = notification.Url
            }, options => { options.File = "user-registered-confirmation.html"; });

            await _emailer.Send(user.Email, _options.ConfirmationEmail.Subject, confirmationContent);

        }
    }
}
