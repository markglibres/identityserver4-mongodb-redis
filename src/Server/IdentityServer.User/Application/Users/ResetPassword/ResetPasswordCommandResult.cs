using System.Collections.Generic;

namespace IdentityServer.Management.Application.Users.ResetPassword
{
    public class ResetPasswordCommandResult
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
