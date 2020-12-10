using IdentityServer.Repositories;

namespace IdentityServer
{
    public class IdentityServerConfig
    {
        public string Authority { get; set; }
        public IdentityMongoOptions Mongo { get; set; }
        public IdentityRedisOptions Redis { get; set; }
    }
}
