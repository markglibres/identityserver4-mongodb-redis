using IdentityServer.Repositories;

namespace IdentityServer
{
    public class IdentityConfig
    {
        public string Authority { get; set; }
        public IdentityMongoOptions Mongo { get; set; }
        public IdentityRedisOptions Redis { get; set; }
    }
}