using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Hosts.Mvc.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}
