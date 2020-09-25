using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IdentityServer.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IList<T>> Find(Expression<Func<T, bool>> expression);
        Task Delete(Expression<Func<T, bool>> predicate);
        Task<T> SingleOrDefault(Expression<Func<T, bool>> expression);
        Task Update(T item, Expression<Func<T, bool>> expression);
        Task Insert(T item);
        Task Insert(IEnumerable<T> items);
    }
}