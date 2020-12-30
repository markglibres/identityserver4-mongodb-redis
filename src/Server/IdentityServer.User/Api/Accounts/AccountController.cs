using System.Threading.Tasks;
using IdentityServer.Management.Users;
using IdentityServer.Management.Users.Abstractions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Management.Api.Accounts
{
    [Route("identity/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IClientStore _clientStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService<ApplicationUser> _userService;

        public AccountController(
            IIdentityServerInteractionService interaction,
            IUserService<ApplicationUser> userService,
            SignInManager<ApplicationUser> signInManager,
            IClientStore clientStore)
        {
            _interaction = interaction;
            _userService = userService;
            _signInManager = signInManager;
            _clientStore = clientStore;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);
            if (context == null || !await _userService.ValidateCredentials(request.Username, request.Password))
                return Unauthorized();

            var user = await _userService.GetByUsername(request.Username);
            await _signInManager.SignInAsync(user, true);

            //return Redirect(request.ReturnUrl);

            return Ok(new
            {
                isSuccess = true,
                username = request.Username,
                password = request.Password,
                returnUrl = context.RedirectUri
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
