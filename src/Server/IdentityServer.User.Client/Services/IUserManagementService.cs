using System.Threading.Tasks;
using IdentityServer.User.Client.Registration;

namespace IdentityServer.User.Client.Services
{
    public interface IUserManagementService
    {
        Task<RegistrationContext> GetRegistrationContext(string redirectUrl);
    }
}