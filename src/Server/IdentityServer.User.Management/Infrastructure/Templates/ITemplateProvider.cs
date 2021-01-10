using System.Threading.Tasks;

namespace IdentityServer.Management.Infrastructure.Templates
{
    public interface ITemplateProvider
    {
        Task<string> GetContents(string filename);
    }
}
