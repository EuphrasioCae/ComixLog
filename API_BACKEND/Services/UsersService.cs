using ComixLog.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace ComixLog.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService(IOptions<ComixLogDatabaseSettings> ComixLogDatabaseSettings)
        {
            var mongoClient = new MongoClient(ComixLogDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(ComixLogDatabaseSettings.Value.DatabaseName);
            _usersCollection = mongoDatabase.GetCollection<User>(ComixLogDatabaseSettings.Value.UsersCollectionName);
        }

        public virtual async Task<User?> GetByUsernameAsync(string username)
        {
            return await _usersCollection
                .Find(user => user.Name == username || user.EmailAddress == username)
                .FirstOrDefaultAsync();
        }


        // Torne os métodos virtuais
        public virtual async Task<List<User>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public virtual async Task<User> GetAsync(string id) =>
            await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public virtual async Task CreateAsync(User newUser) =>
            await _usersCollection.InsertOneAsync(newUser);

        public virtual async Task UpdateAsync(string id, User updatedUser) =>
            await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        public virtual async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x.Id == id);
    }
}
