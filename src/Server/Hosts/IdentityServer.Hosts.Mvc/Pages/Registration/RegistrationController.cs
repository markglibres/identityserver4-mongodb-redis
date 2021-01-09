using System.Threading.Tasks;
using IdentityServer.Hosts.Mvc.ViewModels;
using IdentityServer.Services.Abstractions;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Hosts.Mvc.Controllers
{
    [AllowAnonymous]
    public class RegistrationController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientSecretValidator _clientSecretValidator;
        private readonly IClientService _clientService;

        public RegistrationController(IIdentityServerInteractionService interactionService,
            IClientSecretValidator clientSecretValidator)
        {
            _interactionService = interactionService;
            _clientSecretValidator = clientSecretValidator;

        }

        [HttpGet]
        public async Task<IActionResult> Create(string token)
        {
            return View(new CreateRegistrationModel
            {
                Token = token
            });
        }

    }

    public class CreateRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
