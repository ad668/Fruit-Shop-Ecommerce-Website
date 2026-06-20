using Microsoft.AspNetCore.Mvc;
using OnlineFruitShop.API.Models;
using OnlineFruitShop.Core.Entities;
using OnlineFruitShop.Core.Interfaces;

namespace OnlineFruitShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> _users;
        private readonly IAuthService _authService;

        public AuthController(IRepository<User> users, IAuthService authService)
        {
            _users = users;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existing = (await _users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
            if (existing != null)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Email already registered." });
            }

            var hashed = await _authService.HashPasswordAsync(request.Password);
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashed,
                Role = "Customer"
            };
            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            var token = await _authService.GenerateTokenAsync(user);
            return Ok(new ApiResponse<object> { Success = true, Message = "Registration successful.", Data = new { token } });
        }

        [HttpPost("register-delivery-partner")]
        public async Task<IActionResult> RegisterDeliveryPartner([FromBody] RegisterDeliveryPartnerRequest request)
        {
            var existing = (await _users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
            if (existing != null)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Email already registered." });
            }

            var hashed = await _authService.HashPasswordAsync(request.Password);
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashed,
                Role = "DeliveryPartner"
            };
            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            var token = await _authService.GenerateTokenAsync(user);
            return Ok(new ApiResponse<object> { Success = true, Message = "Delivery Partner registered successfully.", Data = new { token } });
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
        {
            var existing = (await _users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
            if (existing != null)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Email already registered." });
            }

            var hashed = await _authService.HashPasswordAsync(request.Password);
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashed,
                Role = "Admin"
            };
            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            var token = await _authService.GenerateTokenAsync(user);
            return Ok(new ApiResponse<object> { Success = true, Message = "Admin user registered successfully.", Data = new { token } });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = (await _users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
            if (user == null || !await _authService.VerifyPasswordAsync(user.PasswordHash, request.Password))
            {
                return Unauthorized(new ApiResponse<string> { Success = false, Message = "Invalid login credentials." });
            }

            var token = await _authService.GenerateTokenAsync(user);
            return Ok(new ApiResponse<object> { Success = true, Message = "Login successful.", Data = new { token } });
        }
    }
}
