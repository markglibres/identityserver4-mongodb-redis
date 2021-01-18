using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityServer.Authorization.Services.Abstractions;
using IdentityServer.Common;
using IdentityServer.Hosts.Mvc.ViewModels;
using IdentityServer.Users.Management.Api.Users.RegisterUser;
using IdentityServer.Users.Management.Application.Users.RegisterUser;
using IdentityServer.Users.Management.Configs;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IdentityServer.Hosts.Mvc.Controllers
{
    [AllowAnonymous]
    public class RegistrationController : Controller
    {
        private readonly IClientSecretValidator _clientSecretValidator;
        private readonly IClientService _clientService;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ITokenValidator _tokenValidator;
        private readonly IdentityServerUserManagementConfig _options;

        public RegistrationController(IIdentityServerInteractionService interactionService,
            IClientSecretValidator clientSecretValidator,
            IOptions<IdentityServerUserManagementConfig> options,
            ITokenValidator tokenValidator,
            IMapper mapper,
            IMediator mediator)
        {
            _interactionService = interactionService;
            _clientSecretValidator = clientSecretValidator;
            _options = options.Value;
            _tokenValidator = tokenValidator;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser(string token)
        {
            var validationResult = await _tokenValidator.ValidateAccessTokenAsync(token, _options.Scope);
            if (validationResult.IsError) throw new Exception(validationResult.Error);

            return View(new CreateUserRequest
            {
                Token = token
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserRequest request, string button)
        {
            if (!ModelState.IsValid) return View(request);

            var validationResult = await _tokenValidator.ValidateAccessTokenAsync(request.Token, _options.Scope);
            if (validationResult.IsError) throw new Exception(validationResult.Error);

            var command = _mapper.Map<RegisterUserCommand>(request, userCommand => userCommand.PlainTextPassword = request.Password);
            command.ConfirmUrlFormatter = (userId, token) => $"{GetCurrentPath()}/confirm/{userId}/{token}";

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(nameof(request.Validations), string.Join(" ",result.Errors));
                return View(request);
            }

            var model = _mapper.Map<UserCreatedModel>(result, createdModel => createdModel.Email = request.Email);
            return View("UserCreated", model);
        }

        private string GetCurrentPath()
        {
            return $"{GetBaseUrl()}/{Request.RouteValues["controller"]}";
        }

        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}";
        }
    }

    public class CreateUserRequest
    {
        [Required] public string Email { get; set; }

        [Required] public string Token { get; set; }

        [Required] public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        public string Validations { get; set; }
    }
}
