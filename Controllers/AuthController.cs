using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebMangaProject.DTO; // For LoginDto, SignUpDto, RefreshTokenDto
using MangaAPI.Services;
using MangaAPI.DTO;

namespace WebMangaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Email and password are required.");

            var (token, refreshToken, reader) = await _authService.AuthenticateReaderAsync(loginDto.Email, loginDto.Password);
            if (token == null) // means either reader not found or password invalid
                return Unauthorized("Invalid email or password.");

            return Ok(new
            {
                Message = "Login successful",
                Token = token,
                RefreshToken = refreshToken,
                User = new
                {
                    ReaderID = reader.ReaderID,
                    Username = reader.Username,
                    Email = reader.Email
                }
            });
        }

        // POST: api/Auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshDto)
        {
            if (refreshDto == null || refreshDto.ReaderID <= 0 || string.IsNullOrEmpty(refreshDto.RefreshToken))
                return BadRequest("ReaderID and RefreshToken are required.");

            var (newToken, newRefreshToken, reader) = await _authService.RefreshReaderTokenAsync(refreshDto.ReaderID, refreshDto.RefreshToken);
            if (newToken == null) // means invalid or mismatch token
                return Unauthorized("Invalid refresh token.");

            return Ok(new
            {
                Message = "Token renewed successfully",
                Token = newToken,
                RefreshToken = newRefreshToken,
                User = new
                {
                    ReaderID = reader.ReaderID,
                    Username = reader.Username,
                    Email = reader.Email
                }
            });
        }

        // POST: api/Auth/signup
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
        {
            if (signUpDto == null || string.IsNullOrEmpty(signUpDto.Username) ||
                string.IsNullOrEmpty(signUpDto.Email) || string.IsNullOrEmpty(signUpDto.Password))
            {
                return BadRequest("Username, email, and password are required.");
            }

            var result = await _authService.RegisterReaderAsync(signUpDto.Username, signUpDto.Email, signUpDto.Password);
            if (!result)
                return BadRequest("Sign up failed. The username or email may already exist.");

            return Ok("Registration successful.");
        }
    }
}
