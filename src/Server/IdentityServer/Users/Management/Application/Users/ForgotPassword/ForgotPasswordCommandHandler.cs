using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Application.Users.Events.ForgotPasswordRequested;
using IdentityServer.Users.Management.Configs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Users.Management.Application.Users.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordCommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationEventPublisher _eventPublisher;
        private readonly IdentityServerUserManagementConfig _managementOptions;
        private readonly IUrlBuilder _urlBuilder;

        public ForgotPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IApplicationEventPublisher eventPublisher,
            IOptions<IdentityServerUserManagementConfig> managementOptions,
            IUrlBuilder urlBuilder)
        {
            _userManager = userManager;
            _eventPublisher = eventPublisher;
            _managementOptions = managementOptions.Value;
            _urlBuilder = urlBuilder;
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

            _urlBuilder
                .Create(_managementOptions.UserInteractions.BaseUrl)
                .Path(_managementOptions.UserInteractions.ResetPassword)
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
