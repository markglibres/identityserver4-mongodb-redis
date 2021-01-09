using IdentityServer.User.Client.Registration;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Client.Mvc.Controllers
{
    public class RegistrationController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return new RegistrationResult();
        }
    }
}
