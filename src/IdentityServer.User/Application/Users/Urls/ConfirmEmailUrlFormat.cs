namespace IdentityServer.Management.Application.Users.Urls
{
    public class ConfirmEmailUrlFormat
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        public string UrlFormat { get; set; }

        public new string ToString() => UrlFormat
            .Replace($"{{{nameof(UserId)}}}", UserId)
            .Replace($"{{{nameof(Token)}}}", Token);
    }
}
