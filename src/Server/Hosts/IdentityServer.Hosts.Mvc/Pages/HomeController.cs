using IdentityServer.HostServer.Mvc.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
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

        public IActionResult Error()
        {
            var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            return View("Error", new ErrorModel {Message = exceptionHandler.Error.Message});
        }
    }
}