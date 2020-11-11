using Identity.Common.Repositories;
using IdentityServer.Repositories;

namespace IdentityServer
{
    public partial class IdentityConfig
    {
        public string Authority { get; set; }
        public IdentityMongoOptions Mongo { get; set; }
        public IdentityRedisOptions Redis { get; set; }
    }
}