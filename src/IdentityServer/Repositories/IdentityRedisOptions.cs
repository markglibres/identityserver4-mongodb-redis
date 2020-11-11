namespace IdentityServer.Repositories
{
    public class IdentityRedisOptions
    {
        public IdentityRedisOptions()
        {
            ConnectionString = "localhost";
            Db = -1;
            Prefix = "identity";
        }

        public string ConnectionString { get; set; }
        public int Db { get; set; }
        public string Prefix { get; set; }
    }
}