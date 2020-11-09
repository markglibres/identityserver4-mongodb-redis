using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.Users.UpdateUserPassword
{
    public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand, Unit>
    {
        private readonly IAggregateRepository<User, UserId> _userRepository;

        public UpdateUserPasswordCommandHandler(IAggregateRepository<User, UserId> userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<Unit> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = new User(AggregateGuid.Create<UserId>());
            user.UpdatePassword(Password.Create(request.PlainPassword, false));

            await _userRepository.Save(user, cancellationToken);
            
            return Unit.Value;
        }
    }
}