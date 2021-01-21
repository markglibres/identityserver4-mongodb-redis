using System.Threading.Tasks;
using IdentityServer.Users.Management.Application.Abstractions;

namespace IdentityServer.Users.Management.Infrastructure.Templates
{
    public interface ITemplateProvider
    {
        Task<string> GetContents(EmailTemplateOptions options);
        bool CanHandle(EmailTemplateOptions options);
    }
}
