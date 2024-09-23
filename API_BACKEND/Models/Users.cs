using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ComixLog.Models
{
    public class User
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Name { get; set; } = string.Empty;

        public string? EmailAddress { get; set; } = string.Empty;

        public string? Password { get; set; } = string.Empty;

        public bool? Admin { get; set; }

    }

    
}
