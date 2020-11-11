namespace IdentityServer.Services.Abstractions
{
    public interface IInMemorySettings<T> where T : class
    {
        bool IsEnabled();
    }
}