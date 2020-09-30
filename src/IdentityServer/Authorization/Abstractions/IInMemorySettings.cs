namespace IdentityServer.Authorization.Abstractions
{
    public interface IInMemorySettings<T> where T: class
    {
        bool IsEnabled();
    }
}