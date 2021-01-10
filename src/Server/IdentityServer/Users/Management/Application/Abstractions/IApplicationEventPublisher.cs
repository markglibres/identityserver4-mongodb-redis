using System.Threading.Tasks;

namespace IdentityServer.Users.Management.Application.Abstractions
{
    public interface IApplicationEventPublisher
    {
        Task PublishAsync(IApplicationEvent @event);
    }
}
