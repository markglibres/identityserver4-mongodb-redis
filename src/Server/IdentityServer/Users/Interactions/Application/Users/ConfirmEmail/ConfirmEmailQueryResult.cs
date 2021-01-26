using System.Collections.Generic;

namespace IdentityServer.Users.Interactions.Application.Users.ConfirmEmail
{
    public class ConfirmEmailQueryResult
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string ReturnUrl { get; set; }
        public string ResetPasswordToken { get; set; }
    }
}