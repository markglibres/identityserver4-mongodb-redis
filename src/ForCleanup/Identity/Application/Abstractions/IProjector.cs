using Identity.Domain.Abstractions;
using MediatR;

namespace Identity.Application.Abstractions
{
    public interface IProjector<in T> : INotificationHandler<T> where T : INotification, IDomainEvent
    {
        
    }
}