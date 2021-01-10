using System;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Hosts.Mvc.ViewModels;
using IdentityServer.Users.Interactions.Application.Accounts.Login;
using IdentityServer.Users.Interactions.Application.Accounts.Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Hosts.Mvc.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var query = new LoginQuery {ReturnUrl = returnUrl};
            var result = await _mediator.Send(query);

            var response = _mapper.Map<LoginModel>(result);
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string button)
        {
            var command = _mapper.Map<LoginCommand>(model);
            var commandResult = await _mediator.Send(command);

            if (!commandResult.IsSuccess)
                ModelState.AddModelError(string.Empty, "An error occured while logging in. Please try again.");

            //native client
            if (!commandResult.ReturnUrl.StartsWith("https", StringComparison.Ordinal)
                && !commandResult.ReturnUrl.StartsWith("http", StringComparison.Ordinal))
            {
                HttpContext.Response.StatusCode = 200;
                HttpContext.Response.Headers["Location"] = "";

                return Redirect(commandResult.ReturnUrl);
            }

            var query = new LoginQuery {ReturnUrl = model.ReturnUrl};
            var queryResult = await _mediator.Send(query);
            var response = _mapper.Map<LoginModel>(queryResult);

            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var command = new LogoutCommand
            {
                LogoutId = logoutId,
                Identity = User?.Identity
            };
            var result = await _mediator.Send(command);

            var vm = _mapper.Map<LoggedoutModel>(result);
            return View("LoggedOut", vm);
        }
    }
}
