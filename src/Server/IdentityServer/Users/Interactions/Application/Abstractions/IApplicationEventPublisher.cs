using System.Threading.Tasks;

namespace IdentityServer.Users.Interactions.Application.Abstractions
{
    public interface IApplicationEventPublisher
    {
        Task PublishAsync(IApplicationEvent @event);
    }
}