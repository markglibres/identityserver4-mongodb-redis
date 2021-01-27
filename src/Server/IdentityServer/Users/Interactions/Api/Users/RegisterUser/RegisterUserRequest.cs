namespace IdentityServer.Users.Interactions.Api.Users.RegisterUser
{
    public class RegisterUserRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PlainTextPassword { get; set; }
    }
}