using System.Collections.Generic;

namespace IdentityServer.Management.Api.Requests
{
    public class CreateUserResponse
    {
        public string Id { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}