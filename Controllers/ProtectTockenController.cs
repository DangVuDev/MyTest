using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers
{
    [Authorize]  
    [ApiController]
    [Route("api/[controller]")]
    public class ProtectedController : ControllerBase
    {
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public IActionResult AdminOnlyMethod()
        {
            return Ok("This is only for Admins!");
        }

    }
}
