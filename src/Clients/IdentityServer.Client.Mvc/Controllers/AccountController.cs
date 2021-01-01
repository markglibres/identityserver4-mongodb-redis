using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Client.Mvc.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            //await HttpContext.SignOutAsync("Cookies");
           //await HttpContext.SignOutAsync("oidc");
            return new SignOutResult(new List<string>
            {
                "Cookies",
                "oidc"
            });
        }
    }
}
