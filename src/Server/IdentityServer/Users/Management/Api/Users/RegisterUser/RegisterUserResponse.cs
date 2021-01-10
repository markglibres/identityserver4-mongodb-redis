using System.Collections.Generic;

namespace IdentityServer.Users.Management.Api.Users.RegisterUser
{
    public class RegisterUserResponse
    {
        public string Id { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
