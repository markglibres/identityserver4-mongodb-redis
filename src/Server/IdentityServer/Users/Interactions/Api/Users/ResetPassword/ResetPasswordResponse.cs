using System.Collections.Generic;

namespace IdentityServer.Users.Interactions.Api.Users.ResetPassword
{
    public class ResetPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}