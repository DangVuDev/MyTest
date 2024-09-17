using Microsoft.AspNetCore.Mvc;
using MyAPI.Services;

namespace MyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly ImageServices _imageServices;

        public FileUploadController(ImageServices imageServices)
        {
            _imageServices = imageServices;
        }

        // POST: Upload an image
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var imageUrl = await _imageServices.UploadImageAsync(file);
            if (string.IsNullOrEmpty(imageUrl))
                return BadRequest("Image upload failed.");

            return Ok(new { Url = imageUrl });
        }

        // GET: Get image by URL from MongoDB
        [HttpGet("getimage")]
        public async Task<IActionResult> GetImageByUrl(string url)
        {
            var imageRecord = await _imageServices.GetImageByUrlAsync(url);
            if (imageRecord == null)
                return NotFound("Image not found.");

            return Ok(imageRecord);
        }

        // DELETE: Delete an image by publicId
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage(string publicId)
        {
            var isDeleted = await _imageServices.DeleteImageAsync(publicId);
            if (!isDeleted)
                return BadRequest("Failed to delete the image.");

            return Ok("Image deleted successfully.");
        }
    }
}
