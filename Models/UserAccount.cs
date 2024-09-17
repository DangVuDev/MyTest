using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyAPI.Models
{
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; } = string.Empty;

        public string? Username { get; set; }
        public string? Password { get; set; }
     
    }

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
    

        public string Id { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty;
        public string? Sotaikhoan { get; set; }
        public decimal Balance { get; set; }
    }
}

