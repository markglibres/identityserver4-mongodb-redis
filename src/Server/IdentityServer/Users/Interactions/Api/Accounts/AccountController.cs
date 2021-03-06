using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Abstractions;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Interactions.Application.Accounts.Login;
using IdentityServer.Users.Interactions.Application.Accounts.Logout;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Users.Interactions.Api.Accounts
{
    [Route("identity/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService<ApplicationUser> _userService;

        public AccountController(
            IIdentityServerInteractionService interaction,
            IUserService<ApplicationUser> userService,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            IMediator mediator)
        {
            _interaction = interaction;
            _userService = userService;
            _signInManager = signInManager;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = _mapper.Map<LoginCommand>(request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess) return Unauthorized();

            return Ok(new
            {
                isSuccess = result.IsSuccess,
                username = request.Username,
                password = request.Password,
                returnUrl = result.ReturnUrl
            });
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var command = _mapper.Map<LogoutCommand>(request, logoutCommand => logoutCommand.Identity = User?.Identity);
            var result = await _mediator.Send(command);

            return Ok(new
            {
                result.PostLogoutRedirectUri, result.ClientName
            });
        }
    }
}