using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyAPI.Models
{
    public class ImageUrl
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Url { get; set; } = string.Empty; 
    }
}
