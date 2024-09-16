using MongoDB.Bson;
using MongoDB.Driver;
using MyAPI.Models;

namespace MyAPI.Services{
    public class ImageServices{
        private readonly IMongoCollection<ImageUrl> _collection;
        public ImageServices(IMongoClient mongoClient, IConfiguration configuration){
            var databaseName = configuration["DatabaseSettings:DatabaseName"];
            _collection = mongoClient.GetDatabase(databaseName).GetCollection<ImageUrl>("ImageUrl");
        }
        public async Task AddImageAsync(ImageUrl image)
        {
            await _collection.InsertOneAsync(image);
        }

        // Tìm ảnh theo URL
        public async Task<ImageUrl> GetImageByUrlAsync(string url)
        {
            return await _collection.Find(img => img.Url == url).FirstOrDefaultAsync();
        }
    
    }
}