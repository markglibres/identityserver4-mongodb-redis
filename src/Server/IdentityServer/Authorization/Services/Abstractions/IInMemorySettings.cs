namespace IdentityServer.Authorization.Services.Abstractions
{
    public interface IInMemorySettings<T> where T : class
    {
        bool IsEnabled();
    }
}