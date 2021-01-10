using System.Collections.Generic;

namespace IdentityServer.Users.Management.Application.Users.RegisterUser
{
    public class RegisterUserCommandResult
    {
        public string Id { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
