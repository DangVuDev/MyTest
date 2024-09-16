using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic; // For List
using Microsoft.AspNetCore.StaticFiles; // For file type checking
using MyAPI.Services;  // Sử dụng dịch vụ cho MongoDB
using MyAPI.Models;    // Sử dụng mô hình cho MongoDB

namespace MyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly ImageServices _imservices;

        // Inject ImageServices để thao tác với MongoDB
        public FileUploadController(ImageServices imservices)
        {
            _imservices = imservices;
        }

        // POST: Upload an image
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            // Kiểm tra file có null hoặc rỗng không
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            // Kiểm tra file có phải là định dạng ảnh không
            var supportedTypes = new List<string> { "image/jpeg", "image/png", "image/gif" };
            if (!supportedTypes.Contains(file.ContentType))
            {
                return BadRequest("Only images (JPEG, PNG, GIF) are allowed.");
            }

            // Định nghĩa thư mục để lưu ảnh
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);  // Tạo thư mục nếu chưa có
            }

            // Tạo đường dẫn file để lưu ảnh
            var filePath = Path.Combine(uploadFolder, file.FileName);

            // Lưu file vào hệ thống
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Tạo URL để truy cập file ảnh
            var fileUrl = $"{Request.Scheme}://{Request.Host}/Images/{file.FileName}";

            // Lưu URL vào cơ sở dữ liệu MongoDB
            var imageUrl = new ImageUrl
            {
                Url = fileUrl
            };
            await _imservices.AddImageAsync(imageUrl); // Gọi service để lưu vào MongoDB

            // Trả về kết quả URL của ảnh
            return Ok(new { Url = fileUrl });
        }

        // GET: Lấy ảnh bằng URL
        [HttpGet("getimage")]
        public async Task<IActionResult> GetImageByUrl([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL is required.");
            }

            // Tìm ảnh từ MongoDB theo URL
            var imageRecord = await _imservices.GetImageByUrlAsync(url);
            if (imageRecord == null)
            {
                return NotFound("Image not found in database.");
            }

            // Lấy đường dẫn file từ hệ thống
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", Path.GetFileName(url));

            // Kiểm tra file có tồn tại trên server không
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image not found on server.");
            }

            // Lấy loại MIME của file
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream"; // Nếu không xác định được, trả về octet-stream
            }

            // Đọc file và trả về dưới dạng kết quả file
            var image = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(image, contentType);
        }
    }
}
