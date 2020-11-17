namespace IdentityServer.Management.Api.Requests
{
    public class CreateUserRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PlainTextPassword { get; set; }
    }
}