using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions;
using Identity.Domain.User.Events;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Users.GetUser
{
    public class UserProjection : IProjector<UserCreatedEvent>
    {
        private readonly ILogger<UserProjection> _logger;
        private readonly IDocumentRepository<UserModel> _documentRepository;

        public UserProjection(
            ILogger<UserProjection> logger,
            IDocumentRepository<UserModel> documentRepository)
        {
            _logger = logger;
            _documentRepository = documentRepository;
        }
        
        public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            var model = new UserModel
            {
                Id = notification.EntityId,
                Email = notification.Email,
                Firstname = notification.Firstname,
                Lastname = notification.Lastname,
                Password = notification.Password
            };

            await _documentRepository.Insert(model, TenantId.Create(notification.TenantId));
            _logger.LogInformation($"User {notification.Email} has been projected");
        }
    }
}