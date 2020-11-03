using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.Users.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IAggregateRepository<User, UserId> _userRepository;

        public CreateUserCommandHandler(IAggregateRepository<User, UserId> userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(AggregateGuid.Create<UserId>(request.TenantId));
            user.Create(
                Fullname.Create(request.Firstname, request.Lastname),
                Email.Create(request.Email),
                Password.Create(request.PlainPassword));

            await _userRepository.Save(user, cancellationToken);
            
            return user.Id.Id;
        }
    }
}