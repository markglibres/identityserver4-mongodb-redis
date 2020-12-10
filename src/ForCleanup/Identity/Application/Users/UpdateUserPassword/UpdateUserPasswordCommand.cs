using Identity.Application.Abstractions;
using Identity.Domain.User;
using MediatR;

namespace Identity.Application.Users.UpdateUserPassword
{
    public class UpdateUserPasswordCommand : AggregateCommand<UserId>, IRequest<Unit>
    {
        public string PlainPassword { get; set; }
    }
}