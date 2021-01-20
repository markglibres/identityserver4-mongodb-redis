using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Application.Users.Events.ForgotPasswordRequested;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Users.Management.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordCommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationEventPublisher _eventPublisher;

        public ForgotPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IApplicationEventPublisher eventPublisher)
        {
            _userManager = userManager;
            _eventPublisher = eventPublisher;
        }

        public async Task<ForgotPasswordCommandResult> Handle(ForgotPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return new ForgotPasswordCommandResult
            {
                IsSuccess = false,
                Message = $"User {request.Email} not found."
            };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Base64UrlEncoder.Encode(token);

            var resetUrl = request.ResetUrlFormatter?.Invoke(user.Id, encodedToken, request.ReturnUrl);

            var forgotPasswordRequestedEvent = new ForgotPasswordRequestedEvent
            {
                UserId = user.Id,
                Url = resetUrl
            };

            await _eventPublisher.PublishAsync(forgotPasswordRequestedEvent);

            return new ForgotPasswordCommandResult
            {
                Token = token,
                IsSuccess = true
            };
        }
    }
}
