using System.Collections.Generic;

namespace IdentityServer.Users.Management.Application.Users.ConfirmEmail
{
    public class ConfirmEmailQueryResult
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

}
