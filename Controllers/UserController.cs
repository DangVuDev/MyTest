using Microsoft.AspNetCore.Mvc;
using MyAPI.Services;
using MyAPI.Models;

namespace MyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserServices _userServices;

        // Inject UserServices through constructor
        public UserController(UserServices userServices)
        {
            _userServices = userServices;
        }
        //
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var user = await _userServices.GetAllUsersAsync();
            return Ok(user);
        }

        // Example GET method to retrieve a user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userServices.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // Example POST method to create a new user
        //[HttpPost]
        // public async Task<IActionResult> CreateUser(string id)
        // {
        //     var a = await _userServices.CreateUserAsync(user);
            
        //     // Sửa 'user.id' thành 'user.Id'
        //     return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        // }

        // Example PUT method to update an existing user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var existingUser = await _userServices.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userServices.UpdateUserAsync(id, updatedUser);
            return NoContent();
        }

        // Example DELETE method to delete a user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var existingUser = await _userServices.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userServices.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
