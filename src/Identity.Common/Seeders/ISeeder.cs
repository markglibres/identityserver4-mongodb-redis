using System.Collections.Generic;

namespace Identity.Common.Seeders
{
    public interface ISeeder<out T>
    {
        IEnumerable<T> GetSeeds();
    }
}