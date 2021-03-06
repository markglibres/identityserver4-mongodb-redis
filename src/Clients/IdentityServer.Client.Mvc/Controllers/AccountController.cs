using System.Threading.Tasks;
using IdentityServer.User.Client.Account;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Client.Mvc.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            return new LogoutResult();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return new LoginResult();
        }
    }
}