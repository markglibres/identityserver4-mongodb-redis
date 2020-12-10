using System;
using Identity.Application.Abstractions;
using Identity.Domain.User;
using MediatR;

namespace Identity.Application.Users.CreateUser
{
    public class CreateUserCommand : AggregateCommand<UserId>, IRequest<Guid>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PlainPassword { get; set; }
    }
}