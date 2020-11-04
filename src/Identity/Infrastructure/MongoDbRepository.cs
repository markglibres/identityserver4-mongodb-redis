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
        private MongoDbConfig _options;

        public MongoDbRepository(IOptions<MongoDbConfig> options)
        {
            _options = options.Value;
            _client = new MongoClient(_options.ConnectionString);
        }

        private IMongoDatabase GetDatabase(string database = "") => _client
            .GetDatabase(string.IsNullOrWhiteSpace(database) ? _options.DefaultDatabase : database);

        public async Task<IList<T>> Find(Expression<Func<T, bool>> expression, string database = "")
        {
            var result = await Collection(database).FindAsync(expression);
            return result.ToList();
        }

        public async Task<IList<T>> FindAnyIn(string propertyName, IEnumerable<string> items, string database = "")
        {
            var filter = Builders<T>.Filter.AnyIn(propertyName, items);
            var result = await Collection(database).FindAsync(filter);

            return result.ToList();
        }

        public async Task Delete(Expression<Func<T, bool>> predicate, string database = "")
        {
            await Collection(database).DeleteManyAsync(predicate);
        }

        public async Task<T> SingleOrDefault(Expression<Func<T, bool>> expression, string database = "")
        {
            var result = await Collection(database).FindAsync(expression);

            return result?.SingleOrDefault();
        }

        public async Task Update(T item, Expression<Func<T, bool>> expression, string database = "")
        {
            await Collection(database).FindOneAndReplaceAsync(expression, item, new FindOneAndReplaceOptions<T>(){ IsUpsert = true });
        }

        public async Task Insert(T item, string database = "")
        {
            await Collection(database).InsertOneAsync(item);
        }

        public async Task Insert(IEnumerable<T> items, string database = "")
        {
            await Collection(database).InsertManyAsync(items);
        }

        private IMongoCollection<T> Collection(string database) => GetDatabase(database)?.GetCollection<T>(typeof(T).Name.Camelize());
    }
}