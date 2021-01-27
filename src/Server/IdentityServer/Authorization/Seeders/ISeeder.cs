using System.Collections.Generic;

namespace IdentityServer.Authorization.Seeders
{
    public interface ISeeder<out T>
    {
        IEnumerable<T> GetSeeds();
    }
}