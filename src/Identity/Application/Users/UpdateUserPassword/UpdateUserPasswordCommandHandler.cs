using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions;
using Identity.Domain.User;
using IdentityServer.Management.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Users.UpdateUserPassword
{
    public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand, Unit>
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IAggregateRepository<User, UserId> _userRepository;
        private readonly IUserStore<ApplicationUser> _userStore;

        public UpdateUserPasswordCommandHandler(
            IAggregateRepository<User, UserId> userRepository,
            IUserStore<ApplicationUser> userStore,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userRepository = userRepository;
            _userStore = userStore;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindByIdAsync(request.Id.ToString(), cancellationToken);
            user.PasswordHash = _passwordHasher.HashPassword(user, request.PlainPassword);

            await _userStore.UpdateAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
}