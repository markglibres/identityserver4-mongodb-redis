using System.Threading.Tasks;
using IdentityServer.Client.Mvc.Models;
using IdentityServer.User.Client.Registration;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Client.Mvc.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IUserInteractionService _userInteractionService;

        public RegistrationController(IUserInteractionService userInteractionService)
        {
            _userInteractionService = userInteractionService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return new RegistrationResult();
        }
    }
}
