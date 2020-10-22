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
        private readonly IAggregateRepository<UserAggregate, UserId> _userRepository;

        public CreateUserCommandHandler(IAggregateRepository<UserAggregate, UserId> userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new UserAggregate(AggregateGuid.For<UserId>(request.TenantId));
            user.Create(
                new Fullname(request.Firstname, request.Lastname),
                new Email(request.Email),
                new Password(request.PlainPassword));

            await _userRepository.Save(user, cancellationToken);
            
            return user.Id.Id;
        }
    }
}