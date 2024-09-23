using ComixLog.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace ComixLog.Services
{
    public class ContainersService
    {
        private readonly IMongoCollection<Container> _containersCollection;

        public ContainersService(
            IOptions<ComixLogDatabaseSettings> ComixLogDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                ComixLogDatabaseSettings.Value.ConnectionString);
           
            var mongoDatabase = mongoClient.GetDatabase(
                ComixLogDatabaseSettings.Value.DatabaseName);

            _containersCollection = mongoDatabase.GetCollection<Container>(
                ComixLogDatabaseSettings.Value.ContainersCollectionName);
        }
        public async Task<List<Container>> GetAsync() =>
            await _containersCollection.Find(_=>true).ToListAsync();

        public async Task<Container> GetAsync(string id) =>
            await _containersCollection.Find(x => x.Id ==  id).FirstOrDefaultAsync();

        public async Task CreateAsync(Container newContainer) =>
            await _containersCollection.InsertOneAsync(newContainer);

        public async Task UpdateAsync(string id, Container updatedContainer) =>
            await _containersCollection.ReplaceOneAsync(x => x.Id == id, updatedContainer);

        public async Task RemoveAsync(string id) =>
            await _containersCollection.DeleteOneAsync(x => x.Id == id);
    }


}
