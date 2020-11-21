using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Management.Application.Abstractions;
using IdentityServer.Management.Application.Users.Events;
using IdentityServer.Management.Application.Users.UserRegistered.Templates;
using IdentityServer.Management.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Management.Application.Users.UserRegistered
{
    public class SendUserConfirmationEmailHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IdentityUserConfig _options;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IUserStore<ApplicationUser> _userStore;
        private IEmailer _emailer;

        public SendUserConfirmationEmailHandler(
            IOptions<IdentityUserConfig> options,
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
            var confirmationEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var token = Base64UrlEncoder.Encode(confirmationEmailToken);
            var url = $"{_options.BaseUrl}/{user.Id}/confirm?token={token}";

            var confirmationContent = await _emailTemplate.Generate(new ConfirmationEmailModel
            {
                Url = url
            }, options => { options.File = "user-registered-confirmation.html"; });

            await _emailer.Send(user.Email, _options.ConfirmationEmail.Subject, confirmationContent);

        }
    }
}
