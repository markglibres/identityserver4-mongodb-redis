using System.Threading.Tasks;
using IdentityServer.Hosts.Mvc.ViewModels;
using IdentityServer.Management.Users;
using IdentityServer.Services.Abstractions;
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
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientSecretValidator _clientSecretValidator;
        private readonly IOptions<IdentityServerUserManagementConfig> _options;
        private readonly IClientService _clientService;

        public RegistrationController(IIdentityServerInteractionService interactionService,
            IClientSecretValidator clientSecretValidator,
            IOptions<IdentityServerUserManagementConfig> options)
        {
            _interactionService = interactionService;
            _clientSecretValidator = clientSecretValidator;
            _options = options;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string token)
        {
            return View(new CreateRegistrationModel
            {
                Token = _options.Value.Paths.CreateUserPath
            });
        }

    }

    public class CreateRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
