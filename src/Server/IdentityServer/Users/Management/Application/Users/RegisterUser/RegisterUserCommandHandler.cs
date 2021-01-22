using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IdentityServer.Authorization;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Abstractions;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Application.Users.Events.UserRegistered;
using IdentityServer.Users.Management.Configs;
using IdentityServer4.Services;
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
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IUrlBuilder _urlBuilder;

        private readonly IdentityServerUserManagementConfig _managementOptions;

        public RegisterUserCommandHandler(
            ILogger<RegisterUserCommandHandler> logger,
            UserManager<ApplicationUser> userManager,
            IUserService<ApplicationUser> userService,
            IOptions<IdentityServerUserManagementConfig> managementOptions,
            IOptions<IdentityServerConfig> options,
            IApplicationEventPublisher eventPublisher,
            ITokenValidator tokenValidator,
            IIdentityServerInteractionService interactionService,
            IUrlBuilder urlBuilder)
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _options = options.Value;
            _eventPublisher = eventPublisher;
            _tokenValidator = tokenValidator;
            _interactionService = interactionService;
            _urlBuilder = urlBuilder;
            _managementOptions = managementOptions.Value;
        }

        public async Task<RegisterUserCommandResult> Handle(RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.Token))
            {
                var validationResult = await _tokenValidator.ValidateAccessTokenAsync(request.Token);
                if(validationResult.IsError) throw new DomainException(validationResult.Error);

            } else if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            {
                var context = await _interactionService.GetAuthorizationContextAsync(request.ReturnUrl);
                if(context == null) throw new DomainException("Invalid context");
            }

            var user = await _userService.GetByUsername(request.Email, cancellationToken);
            if (user != null) return new RegisterUserCommandResult
            {
                IsSuccess = false,
                Errors = new List<string>
                {
                    $"Email {request.Email} already exists."
                }
            };

            user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = !_options.RequireConfirmedEmail
            };

            var password = !string.IsNullOrWhiteSpace(request.PlainTextPassword)
                ? request.PlainTextPassword
                : Guid.NewGuid().ToString().ToSha256();

            var result = await _userManager.CreateAsync(user, password);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Base64UrlEncoder.Encode(token);

            var response = new RegisterUserCommandResult
            {
                Id = result.Succeeded ? user.Id : string.Empty,
                IsSuccess = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };

            if (!result.Succeeded) return response;

            _urlBuilder
                .Create(_managementOptions.Routes.BaseUrl)
                .Path(_managementOptions.Routes.ConfirmUser)
                .AddQuery("userId", user.Id)
                .AddQuery("token", encodedToken)
                .AddQuery("returnUrl", request.ReturnUrl);

            var userRegisteredEvent = new UserRegisteredEvent
            {
                UserId = user.Id,
                Url = _urlBuilder.ToString()
            };

            await _eventPublisher.PublishAsync(userRegisteredEvent);
            return response;

        }
    }
}
