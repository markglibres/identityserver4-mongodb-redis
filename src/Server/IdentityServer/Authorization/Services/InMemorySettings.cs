using IdentityServer.Authorization.Services.Abstractions;

namespace IdentityServer.Authorization.Services
{
    public class InMemorySettings<T> : IInMemorySettings<T>
        where T : class
    {
        public bool IsEnabled()
        {
            return true;
        }
    }
}
