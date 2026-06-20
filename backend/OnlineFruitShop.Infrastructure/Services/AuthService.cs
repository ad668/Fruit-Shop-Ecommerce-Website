using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineFruitShop.Core.Entities;
using OnlineFruitShop.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnlineFruitShop.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<bool> VerifyPasswordAsync(string hashedPassword, string password)
        {
            var bytes = Convert.FromBase64String(hashedPassword);
            using var hmac = new HMACSHA512(bytes.Take(128).ToArray());
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Task.FromResult(bytes.Skip(128).SequenceEqual(computedHash));
        }

        public Task<string> HashPasswordAsync(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var combined = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);
            return Task.FromResult(Convert.ToBase64String(combined));
        }

        public Task<string> GenerateTokenAsync(User user)
        {
            var secret = _configuration["Jwt:Secret"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
