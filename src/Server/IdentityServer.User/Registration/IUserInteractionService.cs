using System.Threading.Tasks;

namespace IdentityServer.Management.Registration
{
    public interface IUserInteractionService
    {
        Task<RegistrationContext> GetRegistrationContext();
    }
}
