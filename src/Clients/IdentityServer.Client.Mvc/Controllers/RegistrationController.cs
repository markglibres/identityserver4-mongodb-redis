using IdentityServer.User.Client.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Client.Mvc.Controllers
{
    public class RegistrationController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return new RegistrationResult("/Registration/New");
        }

        [HttpGet]
        [Authorize]
        public IActionResult New()
        {
            return View();
        }
    }
}
