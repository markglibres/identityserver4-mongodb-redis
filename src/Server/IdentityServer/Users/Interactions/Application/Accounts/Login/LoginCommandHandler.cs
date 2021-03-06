using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Users.Authorization.Abstractions;
using IdentityServer.Users.Authorization.Services;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users.Interactions.Application.Accounts.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult>
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService<ApplicationUser> _userService;

        public LoginCommandHandler(IIdentityServerInteractionService interaction,
            IUserService<ApplicationUser> userService,
            SignInManager<ApplicationUser> signInManager)
        {
            _interaction = interaction;
            _userService = userService;
            _signInManager = signInManager;
        }

        public async Task<LoginCommandResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);
            if (context == null || !await _userService.ValidateCredentials(request.Username, request.Password))
                return new LoginCommandResult {IsSuccess = false, ErrorCode = LoginErrorCode.InvalidCredentials};

            if (!await _userService.IsActive(request.Username))
                return new LoginCommandResult {IsSuccess = false, ErrorCode = LoginErrorCode.UnconfirmedEmail};

            var user = await _userService.GetByUsername(request.Username);
            await _signInManager.SignInAsync(user, true);

            var result = new LoginCommandResult
            {
                IsSuccess = true,
                ReturnUrl = request.ReturnUrl
            };

            return result;
        }
    }
}