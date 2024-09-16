using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyAPI.Models;
using MyAPI.Services;
namespace MyAPI.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AccountServices _accountServices;
        private readonly UserServices _userServices;
        public AuthController(IConfiguration configuration,AccountServices accountServices,UserServices userServices)
        {
            _configuration = configuration;
            _accountServices = accountServices;
            _userServices = userServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string _username, [FromQuery] string _password)
        {
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                return BadRequest("Invalid request");
            }
            var account = await _accountServices.GetAccountByUsernameAndPasswordAsync(_username,_password);
            if (account == null)
            {
                return BadRequest("Invalid request");
            }

            var user = await _userServices.GetUserByIdAsync(account.Id); 
            if (user != null)
            {
                var token = GenerateJwtToken(_username);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
