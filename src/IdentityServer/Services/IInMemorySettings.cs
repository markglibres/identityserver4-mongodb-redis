using IdentityServer4.Models;

namespace IdentityServer.Services
{
    public interface IInMemorySettings<T> where T: class
    {
        bool IsEnabled();
    }
}