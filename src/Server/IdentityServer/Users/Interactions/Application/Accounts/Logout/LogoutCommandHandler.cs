using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Users.Authorization.Services;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users.Interactions.Application.Accounts.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutCommandResult>
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutCommandHandler(IIdentityServerInteractionService interactionService,
            SignInManager<ApplicationUser> signInManager)
        {
            _interactionService = interactionService;
            _signInManager = signInManager;
        }

        public async Task<LogoutCommandResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var logout = await _interactionService.GetLogoutContextAsync(request.LogoutId);

            if (request.Identity?.IsAuthenticated == true) await _signInManager.SignOutAsync();

            var result = new LogoutCommandResult
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = request.LogoutId
            };

            return result;
        }
    }
}