using System.Collections.Generic;

namespace IdentityServer.Users.Management.Api.Users.ResetPassword
{
    public class ResetPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
