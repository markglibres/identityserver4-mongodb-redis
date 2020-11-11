using IdentityServer.Services.Abstractions;

namespace IdentityServer.Services
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