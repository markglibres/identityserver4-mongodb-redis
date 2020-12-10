namespace IdentityServer.Management.Infrastructure.Config
{
    public class IdentityAudienceConfig
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public Introspection Introspection { get; set; }
        public bool RequireSsl { get; set; }
    }

    public class Introspection
    {
        public string ClientSecret { get; set; }
    }
}
