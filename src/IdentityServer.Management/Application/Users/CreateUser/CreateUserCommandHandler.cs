using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace IdentityServer.Management.Application.Users.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResult>
    {
        public Task<CreateUserCommandResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CreateUserCommandResult
            {
                Id = Guid.NewGuid().ToString()
            });
        }
    }
}