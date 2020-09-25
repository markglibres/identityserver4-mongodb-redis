using IdentityServer4.Models;

namespace IdentityServer.Services
{
    public class InMemorySettings<T> : IInMemorySettings<T> 
        where T: class
    {
        public bool IsEnabled() => true;
    }
}