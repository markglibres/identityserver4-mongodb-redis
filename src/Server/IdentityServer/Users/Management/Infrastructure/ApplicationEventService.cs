using System.Threading.Tasks;
using IdentityServer.Users.Management.Application.Abstractions;
using MediatR;

namespace IdentityServer.Users.Management.Infrastructure
{
    public class ApplicationEventService : IApplicationEventPublisher
    {
        private readonly IMediator _mediator;

        public ApplicationEventService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishAsync(IApplicationEvent @event)
        {
            await _mediator.Publish(@event);
        }
    }
}
