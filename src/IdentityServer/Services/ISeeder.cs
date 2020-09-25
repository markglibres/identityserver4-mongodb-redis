using System.Collections.Generic;

namespace IdentityServer.Services
{
    public interface ISeeder<out T> where T : class
    {
        IEnumerable<T> GetSeeds();
    }
}