using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyAPI.Models
{
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; } 

        public string? Username { get; set; }
        public string? Password { get; set; }
     
    }

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string? Name { get; set; }
        public string? Sotaikhoan { get; set; }
        public decimal Balance { get; set; }
    }
}

