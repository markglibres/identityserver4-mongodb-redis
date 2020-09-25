using System.Collections.Generic;

namespace IdentityServer
{
    public interface ISeeder<out T> where T: class
    {
        IEnumerable<T> GetSeeds();
    }
}