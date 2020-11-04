using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Humanizer;
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

        public MongoDbRepository(IOptions<MongoDbConfig> options)
        {
            _client = new MongoClient(options.Value.ConnectionString);
        }

        private IMongoDatabase GetDatabase(string databaseName) => _client.GetDatabase(databaseName);

        public async Task<IList<T>> Find(string database, Expression<Func<T, bool>> expression)
        {
            var result = await Collection(database).FindAsync(expression);
            return result.ToList();
        }

        public async Task<IList<T>> FindAnyIn(string database, string propertyName, IEnumerable<string> items)
        {
            var filter = Builders<T>.Filter.AnyIn(propertyName, items);
            var result = await Collection(database).FindAsync(filter);

            return result.ToList();
        }

        public async Task Delete(string database, Expression<Func<T, bool>> predicate)
        {
            await Collection(database).DeleteManyAsync(predicate);
        }

        public async Task<T> SingleOrDefault(string database, Expression<Func<T, bool>> expression)
        {
            var result = await Collection(database).FindAsync(expression);

            return result?.SingleOrDefault();
        }

        public async Task Update(string database, T item, Expression<Func<T, bool>> expression)
        {
            await Collection(database).FindOneAndReplaceAsync(expression, item, new FindOneAndReplaceOptions<T>(){ IsUpsert = true });
        }

        public async Task Insert(string database, T item)
        {
            await Collection(database).InsertOneAsync(item);
        }

        public async Task Insert(string database, IEnumerable<T> items)
        {
            await Collection(database).InsertManyAsync(items);
        }

        private IMongoCollection<T> Collection(string database) => GetDatabase(database)?.GetCollection<T>(typeof(T).Name.Camelize());
    }
}