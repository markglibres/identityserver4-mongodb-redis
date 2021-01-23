using System.Threading.Tasks;

namespace IdentityServer.User.Client.Registration
{
    public interface IUserManagementService
    {
        Task<RegistrationContext> GetRegistrationContext();
    }
}
