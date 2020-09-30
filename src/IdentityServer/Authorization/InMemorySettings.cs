using IdentityServer.Authorization.Abstractions;

namespace IdentityServer.Authorization
{
    public class InMemorySettings<T> : IInMemorySettings<T> 
        where T: class
    {
        public bool IsEnabled() => true;
    }
}