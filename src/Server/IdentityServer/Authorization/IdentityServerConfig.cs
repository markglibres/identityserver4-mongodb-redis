using IdentityServer.Common.Repositories;

namespace IdentityServer.Authorization
{
    public class IdentityServerConfig
    {
        public string Authority { get; set; }
        public IdentityMongoOptions Mongo { get; set; }
        public IdentityRedisOptions Redis { get; set; }
        public bool RequireConfirmedEmail { get; set; }
    }
}