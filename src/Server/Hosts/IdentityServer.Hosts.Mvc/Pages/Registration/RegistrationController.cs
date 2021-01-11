using System;
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

            return View(new CreateUserModel
            {
                Token = token
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserRequest request, string button)
        {
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
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
