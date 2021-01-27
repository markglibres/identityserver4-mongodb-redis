using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Humanizer;
using IdentityServer.Authorization;
using IdentityServer.Common.Repositories.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace IdentityServer.Common.Repositories
{
    public class IdentityMongoRepository<T> : IIdentityRepository<T>
        where T : class, new()
    {
        private readonly IMongoDatabase _database;

        public IdentityMongoRepository(IOptions<IdentityServerConfig> options)
        {
            var client = new MongoClient(options.Value.Mongo.ConnectionString);
            _database = client.GetDatabase(options.Value.Mongo.Database);
        }

        public async Task<IList<T>> Find(Expression<Func<T, bool>> expression)
        {
            var result = await Collection().FindAsync(expression);
            return result.ToList();
        }

        public async Task<IList<T>> FindAnyIn(string propertyName, IEnumerable<string> items)
        {
            var filter = Builders<T>.Filter.AnyIn(propertyName, items);
            var result = await Collection().FindAsync(filter);

            return result.ToList();
        }

        public async Task Delete(Expression<Func<T, bool>> predicate)
        {
            await Collection().DeleteManyAsync(predicate);
        }

        public async Task<T> SingleOrDefault(Expression<Func<T, bool>> expression)
        {
            var result = await Collection().FindAsync(expression);

            return result?.SingleOrDefault();
        }

        public async Task Update(T item, Expression<Func<T, bool>> expression)
        {
            await Collection().FindOneAndReplaceAsync(expression, item);
        }

        public async Task Insert(T item)
        {
            await Collection().InsertOneAsync(item);
        }

        public async Task Insert(IEnumerable<T> items)
        {
            await Collection().InsertManyAsync(items);
        }

        private IMongoCollection<T> Collection()
        {
            return _database.GetCollection<T>(typeof(T).Name.Camelize());
        }
    }
}