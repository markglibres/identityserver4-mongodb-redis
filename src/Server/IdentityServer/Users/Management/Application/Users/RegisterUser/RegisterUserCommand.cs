using System;
using MediatR;

namespace IdentityServer.Users.Management.Application.Users.RegisterUser
{
    public class RegisterUserCommand : IRequest<RegisterUserCommandResult>
    {
        public string Email { get; set; }
        public string PlainTextPassword { get; set; }
        public ConfirmUrlFormatter ConfirmUrlFormatter { get; set; }
    }

    public delegate string ConfirmUrlFormatter(string userId, string token);

}
