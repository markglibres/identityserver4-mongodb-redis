using System.Threading.Tasks;
using IdentityServer.Users.Interactions.Application.Abstractions;

namespace IdentityServer.Users.Interactions.Infrastructure.Templates
{
    public interface ITemplateProvider
    {
        Task<string> GetContents(EmailTemplateOptions options);
        bool CanHandle(EmailTemplateOptions options);
    }
}