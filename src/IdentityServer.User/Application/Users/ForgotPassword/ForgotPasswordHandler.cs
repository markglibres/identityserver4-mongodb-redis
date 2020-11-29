using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Exceptions;
using IdentityServer.Management.Application.Abstractions;
using IdentityServer.Management.Application.Users.Notifications.ForgotPasswordRequested;
using IdentityServer.Management.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Management.Application.Users.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordCommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationEventPublisher _eventPublisher;

        public ForgotPasswordHandler(
            UserManager<ApplicationUser> userManager,
            IApplicationEventPublisher eventPublisher)
        {
            _userManager = userManager;
            _eventPublisher = eventPublisher;
        }

        public async Task<ForgotPasswordCommandResult> Handle(ForgotPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null) throw new DomainException($"User {request.Id} not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Base64UrlEncoder.Encode(token);

            var forgotPasswordRequestedEvent = new ForgotPasswordRequestedEvent
            {
                UserId = user.Id,
                Url = request.ResetPasswordUrlFormat.Replace("{token}", encodedToken)
            };

            await _eventPublisher.PublishAsync(forgotPasswordRequestedEvent);

            return new ForgotPasswordCommandResult
            {
                Token = token
            };
        }
    }
}
