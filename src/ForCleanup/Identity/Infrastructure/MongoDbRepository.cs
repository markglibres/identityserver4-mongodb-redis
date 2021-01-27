using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Humanizer;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Abstractions;
using Identity.Infrastructure.Configs;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Identity.Infrastructure
{
    public class MongoDbRepository<T> : IDocumentRepository<T>
        where T : class, new()
    {
        private readonly MongoClient _client;
        private MongoDbConfig _options;

        public MongoDbRepository(IOptions<MongoDbConfig> options)
        {
            _options = options.Value;
            _client = new MongoClient(_options.ConnectionString);
        }

        public async Task<IList<T>> Find(Expression<Func<T, bool>> expression, TenantId tenantId = null)
        {
            var result = await Collection(GetDatabase(tenantId)).FindAsync(expression);
            return result.ToList();
        }

        public async Task<IList<T>> FindAnyIn(string propertyName, IEnumerable<string> items, TenantId tenantId = null)
        {
            var filter = Builders<T>.Filter.AnyIn(propertyName, items);
            var result = await Collection(GetDatabase(tenantId)).FindAsync(filter);

            return result.ToList();
        }

        public async Task Delete(Expression<Func<T, bool>> predicate, TenantId tenantId = null)
        {
            await Collection(GetDatabase(tenantId)).DeleteManyAsync(predicate);
        }

        public async Task<T> SingleOrDefault(Expression<Func<T, bool>> expression, TenantId tenantId = null)
        {
            var result = await Collection(GetDatabase(tenantId)).FindAsync(expression);

            return result?.SingleOrDefault();
        }

        public async Task Update(T item, Expression<Func<T, bool>> expression, TenantId tenantId = null)
        {
            await Collection(GetDatabase(tenantId)).FindOneAndReplaceAsync(expression, item);
        }

        public async Task Insert(T item, TenantId tenantId = null)
        {
            await Collection(GetDatabase(tenantId)).InsertOneAsync(item);
        }

        public async Task Insert(IEnumerable<T> items, TenantId tenantId = null)
        {
            await Collection(GetDatabase(tenantId)).InsertManyAsync(items);
        }

        private IMongoCollection<T> Collection(string database) => GetDatabase(database)?.GetCollection<T>(typeof(T).Name.Camelize());
        
        private IMongoDatabase GetDatabase(string database = "") => _client
            .GetDatabase(string.IsNullOrWhiteSpace(database) ? _options.DefaultDatabase : database);
        private string GetDatabase(TenantId tenantId) => tenantId?.ToString() ?? TenantId.Default.ToString();
        
    }
}