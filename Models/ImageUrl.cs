

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MyAPI.Models;
public class ImageUrl
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] // Chuyển đổi ObjectId từ MongoDB sang chuỗi
    public string Id { get; set; }

    public string Url { get; set; }
}

