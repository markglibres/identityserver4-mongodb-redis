using System.Threading;
using System.Threading.Tasks;
using Identity.Common.Users;
using Identity.Common.Users.Abstractions;
using Identity.Domain;
using Identity.Domain.User.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Worker.Integration.IdentityServer
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventHandler> _logger;
        private readonly IUserService<ApplicationUser> _userService;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public UserCreatedEventHandler(
            ILogger<UserCreatedEventHandler> logger,
            IUserService<ApplicationUser> userService,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _logger = logger;
            _userService = userService;
            _passwordHasher = passwordHasher;
        }
        
        public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByUsername(notification.Email, cancellationToken);
            if (user != null)
            {
                _logger.LogError($"User {notification.Email} already exists");
                return;
            }
            
            user = new ApplicationUser
            {
                Id = notification.EntityId,
                Email = notification.Email,
                UserName = notification.Email,
                EmailConfirmed = true,
                PasswordHash =   notification.Password // _passwordHasher.HashPassword(null, notification.Password)
            };

            await _userService.Create(user, cancellationToken);
            _logger.LogError($"User {notification.Email} created");
        }
    }
}