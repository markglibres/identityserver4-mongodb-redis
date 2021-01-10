using System.Threading.Tasks;
using IdentityServer.Management.Application.Accounts.Login;
using IdentityServer.Management.Application.Accounts.Logout;
using IdentityServer.Management.Common;
using IdentityServer.Management.Users;
using IdentityServer.Management.Users.Abstractions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MediatR;
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
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
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
                PostLogoutRedirectUri = result.PostLogoutRedirectUri,
                ClientName = result.ClientName
            });
        }
    }
}
