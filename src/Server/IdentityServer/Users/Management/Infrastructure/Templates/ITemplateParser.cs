using System.Threading.Tasks;

namespace IdentityServer.Users.Management.Infrastructure.Templates
{
    public interface ITemplateParser
    {
        Task<string> Parse(string content, object model);
    }
}
