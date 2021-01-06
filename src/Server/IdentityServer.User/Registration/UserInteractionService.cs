using System.Threading.Tasks;

namespace IdentityServer.Management.Registration
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly UserInteractionOptions _interactionOptions;

        public UserInteractionService(UserInteractionOptions interactionOptions)
        {
            _interactionOptions = interactionOptions;
        }

        public Task<RegistrationContext> GetRegistrationContext()
        {
            return Task.FromResult(new RegistrationContext($"{_interactionOptions.Authority}/identity/registration?cliendId={_interactionOptions.ClientId}"));
        }
    }
}
