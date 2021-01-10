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
        private readonly IOptions<IdentityServerUserManagementConfig> _options;

        public RegistrationController(IIdentityServerInteractionService interactionService,
            IClientSecretValidator clientSecretValidator,
            IOptions<IdentityServerUserManagementConfig> options)
        {
            _interactionService = interactionService;
            _clientSecretValidator = clientSecretValidator;
            _options = options;
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser(string token)
        {
            return View(new CreateUserModel
            {
                Token = token
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserRequest request, string button)
        {
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
