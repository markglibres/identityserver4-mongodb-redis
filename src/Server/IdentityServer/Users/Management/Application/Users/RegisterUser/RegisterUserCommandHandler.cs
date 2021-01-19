using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Authorization;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Abstractions;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Application.Users.Events.UserRegistered;
using IdentityServer.Users.Management.Configs;
using IdentityServer4.Validation;
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
        private readonly IdentityServerConfig _options;
        private readonly IApplicationEventPublisher _eventPublisher;
        private readonly ITokenValidator _tokenValidator;

        private readonly IdentityServerUserManagementConfig _managementOptions;

        public RegisterUserCommandHandler(
            ILogger<RegisterUserCommandHandler> logger,
            UserManager<ApplicationUser> userManager,
            IUserService<ApplicationUser> userService,
            IOptions<IdentityServerUserManagementConfig> managementOptions,
            IOptions<IdentityServerConfig> options,
            IApplicationEventPublisher eventPublisher,
            ITokenValidator tokenValidator)
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _options = options.Value;
            _eventPublisher = eventPublisher;
            _tokenValidator = tokenValidator;
            _managementOptions = managementOptions.Value;
        }

        public async Task<RegisterUserCommandResult> Handle(RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _tokenValidator.ValidateAccessTokenAsync(request.Token);
            if(validationResult.IsError) throw new DomainException(validationResult.Error);

            var clientId = validationResult.Client.ClientId;
            var claimType = $"app_{clientId}";
            var claimValue = true.ToString();

            var user = await _userService.GetByUsername(request.Email, cancellationToken);
            if (user != null)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var hasClaim = claims.Any(c => c.Type == claimType && c.Value == claimValue);

                if (hasClaim)
                {
                    var error = $"User {request.Email} already exists";
                    _logger.LogError(error);
                    return new RegisterUserCommandResult
                    {
                        IsSuccess = false,
                        Errors = new List<string> { error }
                    };
                }

                await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
                var userRegisteredEvent = new UserRegisteredEvent
                {
                    UserId = user.Id,
                    ClientId = clientId
                };

                await _eventPublisher.PublishAsync(userRegisteredEvent);
                return new RegisterUserCommandResult
                {
                    IsSuccess = true
                };
            }
            else
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = request.Email,
                    UserName = request.Email,
                    EmailConfirmed = !_options.RequireConfirmedEmail
                };

                var result = await _userManager.CreateAsync(user, request.PlainTextPassword);
                await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = Base64UrlEncoder.Encode(token);
                var confirmationUrl = request.ConfirmUrlFormatter?.Invoke(user.Id, encodedToken);

                var response = new RegisterUserCommandResult
                {
                    Id = result.Succeeded ? user.Id : string.Empty,
                    IsSuccess = result.Succeeded,
                    Errors = result.Errors.Select(e => e.Description)
                };

                if (!result.Succeeded) return response;
                var userRegisteredEvent = new UserRegisteredEvent
                {
                    UserId = user.Id,
                    Url = confirmationUrl,
                    ClientId = clientId
                };

                await _eventPublisher.PublishAsync(userRegisteredEvent);
                return response;
            }

        }
    }
}
