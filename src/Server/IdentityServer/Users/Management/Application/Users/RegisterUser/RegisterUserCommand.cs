using System;
using MediatR;

namespace IdentityServer.Users.Management.Application.Users.RegisterUser
{
    public class RegisterUserCommand : IRequest<RegisterUserCommandResult>
    {
        public string ReturnUrl { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string PlainTextPassword { get; set; }
    }

}
