using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Management.Users;
using IdentityServer.Management.Users.Abstractions;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Management.Application.Accounts.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult>
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IUserService<ApplicationUser> _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;

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
                return new LoginCommandResult { IsSuccess = false };

            var user = await _userService.GetByUsername(request.Username);
            await _signInManager.SignInAsync(user, true);

            var result = new LoginCommandResult
            {
                IsSuccess = true,
                ReturnUrl = context.RedirectUri
            };

            return result;
        }
    }
}
