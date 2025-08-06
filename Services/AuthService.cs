using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic; // For Dictionary<>
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using MangaAPI.Data;
using MangaAPI.Models;
using WebMangaProject.DTO; // For RefreshTokenDto or future DTOs

namespace MangaAPI.Services
{
    // We keep the interface in the same file
    public interface IAuthService
    {
        Task<(string Token, string RefreshToken, Reader Reader)> AuthenticateReaderAsync(string email, string password);
        Task<bool> RegisterReaderAsync(string username, string email, string password);

        // New method to refresh tokens
        Task<(string Token, string RefreshToken, Reader Reader)> RefreshReaderTokenAsync(int readerId, string refreshToken);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // For demo, we store refresh tokens in memory. In production, store in DB or distributed cache.
        private static readonly Dictionary<int, string> _refreshTokens = new Dictionary<int, string>();

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> RegisterReaderAsync(string username, string email, string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $@"EXEC usp_SignUp @Username={username}, @Email={email}, @Password={hashedPassword}");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Authenticates a user by email/password, returning a JWT, a refresh token, and the Reader entity.
        /// </summary>
        public async Task<(string Token, string RefreshToken, Reader Reader)> AuthenticateReaderAsync(string email, string password)
        {
            // Retrieve the reader record by email
            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.Email == email);
            if (reader == null)
                return (null, null, null);

            // Verify the provided password against the stored hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, reader.Password);
            if (!isPasswordValid)
                return (null, null, null);

            // Generate the JWT token
            var token = GenerateJwtToken(reader);
            // Generate a refresh token
            var refreshToken = GenerateRefreshToken();

            // Store the refresh token in memory (per ReaderID)
            _refreshTokens[reader.ReaderID] = refreshToken;

            return (token, refreshToken, reader);
        }

        /// <summary>
        /// Validates the given refresh token and returns a new JWT + refresh token if valid.
        /// </summary>
        public async Task<(string Token, string RefreshToken, Reader Reader)> RefreshReaderTokenAsync(int readerId, string refreshToken)
        {
            // Check if the refresh token matches what we have stored
            if (!_refreshTokens.ContainsKey(readerId) || _refreshTokens[readerId] != refreshToken)
            {
                // Token doesn't match
                return (null, null, null);
            }

            // Retrieve the Reader from DB
            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.ReaderID == readerId);
            if (reader == null)
                return (null, null, null);

            // Generate new tokens
            var newToken = GenerateJwtToken(reader);
            var newRefreshToken = GenerateRefreshToken();

            // Update the in-memory store
            _refreshTokens[readerId] = newRefreshToken;

            return (newToken, newRefreshToken, reader);
        }

        private string GenerateJwtToken(Reader reader)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, reader.Username),
                new Claim("ReaderID", reader.ReaderID.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiresInHours"])),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            // In production, you might use a more robust token generation approach
            return Guid.NewGuid().ToString();
        }
    }
}
