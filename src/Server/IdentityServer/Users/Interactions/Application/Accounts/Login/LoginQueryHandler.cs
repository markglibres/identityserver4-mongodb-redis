using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MediatR;

namespace IdentityServer.Management.Application.Accounts.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginQueryResult>
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientStore _clientStore;

        public LoginQueryHandler(IIdentityServerInteractionService interactionService,
            IClientStore clientStore)
        {
            _interactionService = interactionService;
            _clientStore = clientStore;
        }

        public async Task<LoginQueryResult> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var context = await _interactionService.GetAuthorizationContextAsync(request.ReturnUrl);
            if (context?.Client.ClientId == null) return new LoginQueryResult();

            var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
            var result = new LoginQueryResult
            {
                IsValid = true,
                ReturnUrl = request.ReturnUrl,
                Username = context.LoginHint
            };

            return result;

        }
    }
}
