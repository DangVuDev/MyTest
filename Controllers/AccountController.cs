using Microsoft.AspNetCore.Mvc;
using MyAPI.Services;
using MyAPI.Models;

namespace MyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountServices _accountServices;
        private readonly UserServices _userServices;

        // Inject AccountServices thông qua constructor
        public AccountController(AccountServices accountServices,UserServices userServices)
        {
            _accountServices = accountServices;
            _userServices = userServices;
        }

        // GET: Lấy tài khoản theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var account = await _accountServices.GetAccountByIdAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // GET: Lấy tất cả tài khoản
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountServices.GetAllAccountsAsync();
            return Ok(accounts);
        }

        // POST: Tạo tài khoản mới
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromQuery] string _username, [FromQuery] string _password)
        {
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                return BadRequest("Username or password cannot be empty");
            }

            // Tạo mới Account
            var createdAccount = await _accountServices.CreateAccountAsync(_username, _password);
            // Tạo mới User với Id của Account
            var createUser = await _userServices.CreateUserAsync(createdAccount.Id);

            // Trả về CreatedAtAction với Id của Account
            return CreatedAtAction(nameof(GetAccountById), new { id = createdAccount.Id.ToString() }, createdAccount);
        }


        // PUT: Cập nhật tài khoản theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] Account updatedAccount)
        {
            var existingAccount = await _accountServices.GetAccountByIdAsync(id);
            if (existingAccount == null)
            {
                return NotFound();
            }

            await _accountServices.UpdateAccountAsync(id, updatedAccount);
            return NoContent();
        }

        // DELETE: Xóa tài khoản theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var existingAccount = await _accountServices.GetAccountByIdAsync(id);
            if (existingAccount == null)
            {
                return NotFound();
            }

            await _accountServices.DeleteAccountAsync(id);
            return NoContent();
        }

        // POST: Đăng nhập bằng tài khoản (kiểm tra username và password)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Username and password cannot be empty.");
            }

            var account = await _accountServices.GetAccountByUsernameAndPasswordAsync(loginRequest.Username, loginRequest.Password);
            
            if (account == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(account); // Trả về account hoặc token JWT tùy yêu cầu của bạn
        }

    }

    // Định nghĩa mẫu yêu cầu đăng nhập
    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
