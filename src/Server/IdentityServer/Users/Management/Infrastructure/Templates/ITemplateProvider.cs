using System.Threading.Tasks;

namespace IdentityServer.Users.Management.Infrastructure.Templates
{
    public interface ITemplateProvider
    {
        Task<string> GetContents(string filename);
    }
}
