using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Identity.Domain.ValueObjects;

namespace Identity.Infrastructure.Abstractions
{
    public interface IDocumentRepository<T> where T : class
    {
        Task<IList<T>> Find(Expression<Func<T, bool>> expression, TenantId tenantId = null);
        Task<IList<T>> FindAnyIn(string propertyName, IEnumerable<string> items, TenantId tenantId = null);
        Task Delete(Expression<Func<T, bool>> predicate, TenantId tenantId = null);
        Task<T> SingleOrDefault(Expression<Func<T, bool>> expression, TenantId tenantId = null);
        Task Update(T item, Expression<Func<T, bool>> expression, TenantId tenantId = null);
        Task Insert(T item, TenantId tenantId = null);
        Task Insert(IEnumerable<T> items, TenantId tenantId = null);
    }
}