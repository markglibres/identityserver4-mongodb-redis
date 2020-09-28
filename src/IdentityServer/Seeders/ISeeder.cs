using System.Collections.Generic;

namespace IdentityServer.Services
{
    public interface ISeeder<out T>
    {
        IEnumerable<T> GetSeeds();
    }
}