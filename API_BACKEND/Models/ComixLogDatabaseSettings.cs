using MongoDB.Driver.Core.Configuration;

namespace ComixLog.Models
{
    public class ComixLogDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ContainersCollectionName { get; set; } = null!;

        public string UsersCollectionName { get; set; } = null!;


    }
}
