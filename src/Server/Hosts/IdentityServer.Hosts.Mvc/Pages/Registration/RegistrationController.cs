using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityServer.Authorization.Services.Abstractions;
using IdentityServer.Hosts.Mvc.ViewModels;
using IdentityServer.Users.Management.Configs;
using IdentityServer4.Services;
using IdentityServer4.Validation;
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
        private readonly ITokenValidator _tokenValidator;
        private IdentityServerUserManagementConfig _options;

        public RegistrationController(IIdentityServerInteractionService interactionService,
            IClientSecretValidator clientSecretValidator,
            IOptions<IdentityServerUserManagementConfig> options,
            ITokenValidator tokenValidator)
        {
            _interactionService = interactionService;
            _clientSecretValidator = clientSecretValidator;
            _options = options.Value;
            _tokenValidator = tokenValidator;
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

            return View("UserCreated", new UserCreatedModel
            {
                Email = request.Email
            });
        }
    }

    public class CreateUserRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
    }
}
