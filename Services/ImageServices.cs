using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MongoDB.Driver;
using MyAPI.Models;

namespace MyAPI.Services
{
    public class ImageServices
    {
        private readonly IMongoCollection<ImageUrl> _imageUrls;
        private readonly Cloudinary _cloudinary;

        public ImageServices(IDBMonggoSetting setting,IMongoClient client, Cloudinary cloudinary)
        {
            var database = client.GetDatabase(setting.DatabaseName);
            _imageUrls = database.GetCollection<ImageUrl>("ImageUrls");
            _cloudinary = cloudinary;
        }

        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            // Upload ảnh lên Cloudinary
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Lưu URL của ảnh vào MongoDB
            var imageUrl = new ImageUrl
            {
                Url = uploadResult.SecureUrl.ToString()
            };

            await _imageUrls.InsertOneAsync(imageUrl);

            return imageUrl.Url;
        }

        public async Task<ImageUrl> GetImageByUrlAsync(string url)
        {
            return await _imageUrls.Find(img => img.Url == url).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok";
        }
    }
}
