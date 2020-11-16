namespace IdentityServer.Management.Application.Users.CreateUser
{
    public class CreateUserCommand
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PlainTextPassword { get; set; }
    }
}