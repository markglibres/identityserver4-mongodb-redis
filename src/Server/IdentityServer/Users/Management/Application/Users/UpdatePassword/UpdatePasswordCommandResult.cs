using System.Collections.Generic;

namespace IdentityServer.Users.Management.Application.Users
{
    public class UpdatePasswordCommandResult
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string Token { get; set; }
    }
}
