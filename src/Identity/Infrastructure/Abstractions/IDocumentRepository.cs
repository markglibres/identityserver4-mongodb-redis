using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Abstractions
{
    public interface IDocumentRepository<T> where T : class
    {
        Task<IList<T>> Find(string database, Expression<Func<T, bool>> expression);
        Task<IList<T>> FindAnyIn(string database, string propertyName, IEnumerable<string> items);
        Task Delete(string database, Expression<Func<T, bool>> predicate);
        Task<T> SingleOrDefault(string database, Expression<Func<T, bool>> expression);
        Task Update(string database, T item, Expression<Func<T, bool>> expression);
        Task Insert(string database, T item);
        Task Insert(string database, IEnumerable<T> items);
    }
}