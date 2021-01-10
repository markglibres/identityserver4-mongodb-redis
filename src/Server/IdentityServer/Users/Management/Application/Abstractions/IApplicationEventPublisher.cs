using System.Threading.Tasks;

namespace IdentityServer.Management.Application.Abstractions
{
    public interface IApplicationEventPublisher
    {
        Task PublishAsync(IApplicationEvent @event);
    }
}
