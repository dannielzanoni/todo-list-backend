using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using todo_list.Models;
using todo_list.Services;

namespace todo_list.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var user = await _userService.Authenticate(userDto.Username, userDto.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user.Username);
            return Ok(new { Token = token });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserDto userDto)
        {
            try
            {
                var existingUser = await _userService.GetByUsername(userDto.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                var user = new User { Username = userDto.Username, PasswordHash = userDto.Password };
                await _userService.Register(user);

                return Ok(new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during signup: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred during signup" });
            }
        }


        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
