using System.Collections.Generic;

namespace IdentityServer.Seeders
{
    public interface ISeeder<out T>
    {
        IEnumerable<T> GetSeeds();
    }
}