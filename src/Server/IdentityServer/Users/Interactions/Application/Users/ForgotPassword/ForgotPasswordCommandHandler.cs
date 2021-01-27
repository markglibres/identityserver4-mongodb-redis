using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Interactions.Application.Abstractions;
using IdentityServer.Users.Interactions.Application.Users.Events.ForgotPasswordRequested;
using IdentityServer.Users.Interactions.Infrastructure.Config;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Users.Interactions.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordCommandResult>
    {
        private readonly IApplicationEventPublisher _eventPublisher;
        private readonly IdentityServerUserInteractionConfig _interactionOptions;
        private readonly IUrlBuilder _urlBuilder;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgotPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IApplicationEventPublisher eventPublisher,
            IOptions<IdentityServerUserInteractionConfig> managementOptions,
            IUrlBuilder urlBuilder)
        {
            _userManager = userManager;
            _eventPublisher = eventPublisher;
            _interactionOptions = managementOptions.Value;
            _urlBuilder = urlBuilder;
        }

        public async Task<ForgotPasswordCommandResult> Handle(ForgotPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new ForgotPasswordCommandResult
                {
                    IsSuccess = false,
                    Message = $"User {request.Email} not found."
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Base64UrlEncoder.Encode(token);

            _urlBuilder
                .Create(_interactionOptions.UserInteractionEndpoints.BaseUrl)
                .Path(_interactionOptions.UserInteractionEndpoints.ResetPassword)
                .AddQuery("userId", user.Id)
                .AddQuery("token", encodedToken)
                .AddQuery("returnUrl", request.ReturnUrl);


            var forgotPasswordRequestedEvent = new ForgotPasswordRequestedEvent
            {
                UserId = user.Id,
                Url = _urlBuilder.ToString()
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