namespace IdentityServer.Common
{
    public interface IUrlBuilder
    {
        IUrlBuilder Create(string domain = null);
        IUrlBuilder Path(string path);
        IUrlBuilder AddQuery(string key, string value);
        string ToString();
    }
}