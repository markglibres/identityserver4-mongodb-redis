namespace IdentityServer.Management.Application.Users.RegisterUser
{
    public class ConfirmationEmailUrlFormat
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        public string UrlFormat { get; set; }

        public new string ToString() => UrlFormat
            .Replace($"{{{nameof(UserId)}}}", UserId)
            .Replace($"{{{nameof(Token)}}}", Token);
    }
}
