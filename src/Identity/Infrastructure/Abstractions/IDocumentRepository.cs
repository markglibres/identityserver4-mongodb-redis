using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Abstractions
{
    public interface IDocumentRepository<T> where T : class
    {
        Task<IList<T>> Find(Expression<Func<T, bool>> expression, string database = "");
        Task<IList<T>> FindAnyIn(string propertyName, IEnumerable<string> items, string database = "");
        Task Delete(Expression<Func<T, bool>> predicate, string database = "");
        Task<T> SingleOrDefault(Expression<Func<T, bool>> expression, string database = "");
        Task Update(T item, Expression<Func<T, bool>> expression, string database = "");
        Task Insert(T item, string database = "");
        Task Insert(IEnumerable<T> items, string database = "");
    }
}