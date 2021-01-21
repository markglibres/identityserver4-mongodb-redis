using System.Collections.Generic;

namespace IdentityServer.Users.Management.Api.Users.ConfirmEmail
{
    public class ConfirmEmailResponse
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string ReturnUrl { get; set; }
        public string ResetPasswordToken { get; set; }
    }
}
