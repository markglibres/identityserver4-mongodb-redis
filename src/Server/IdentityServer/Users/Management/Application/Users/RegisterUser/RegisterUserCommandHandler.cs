using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Users.Authorization.Abstractions;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Application.Users.Events.UserRegistered;
using IdentityServer.Users.Management.Configs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Users.Management.Application.Users.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserCommandResult>
    {
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService<ApplicationUser> _userService;
        private readonly IApplicationEventPublisher _eventPublisher;

        private readonly IdentityServerUserManagementConfig _serverUserOptions;

        public RegisterUserCommandHandler(
            ILogger<RegisterUserCommandHandler> logger,
            UserManager<ApplicationUser> userManager,
            IUserService<ApplicationUser> userService,
            IOptions<IdentityServerUserManagementConfig> userOptions,
            IApplicationEventPublisher eventPublisher)
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _serverUserOptions = userOptions.Value;
        }
        public async Task<RegisterUserCommandResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByUsername(request.Email, cancellationToken);
            if(user != null)
            {
                _logger.LogError($"User {request.Email} already exists");
                return new RegisterUserCommandResult
                {
                    IsSuccess = false
                };
            }

            user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = !_serverUserOptions.ConfirmationEmail.Require
            };

            var result = await _userManager.CreateAsync(user, request.PlainTextPassword);
            var response = new RegisterUserCommandResult
            {
                Id = result.Succeeded ? user.Id : string.Empty,
                IsSuccess = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };

            if (!result.Succeeded) return response;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Base64UrlEncoder.Encode(token);

            var confirmationUrl = request.ConfirmUrl;
            confirmationUrl.UserId = user.Id;
            confirmationUrl.Token = encodedToken;

            var userRegisteredEvent = new UserRegisteredEvent
            {
                UserId = user.Id,
                Url = confirmationUrl.ToString()
            };

            await _eventPublisher.PublishAsync(userRegisteredEvent);

            return response;
        }
    }
}
