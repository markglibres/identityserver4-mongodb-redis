using System.Threading.Tasks;

namespace IdentityServer.Users.Interactions.Application.Abstractions
{
    public interface IEmailer
    {
        Task Send(string email, string subject, string content);
    }
}