using System.Threading.Tasks;

namespace IdentityServer.Management.Application.Abstractions
{
    public interface IEmailer
    {
        Task Send(string email, string subject, string content);
    }
}
