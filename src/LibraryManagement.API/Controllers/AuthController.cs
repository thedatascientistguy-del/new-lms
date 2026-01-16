using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository userRepository, IJwtService jwtService, ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            _logger.LogInformation("Signup attempt for email: {Email}", request.Email);
            
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Signup failed: Missing email or password");
                return BadRequest("Email and password are required");
            }

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Signup failed: User {Email} already exists", request.Email);
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            var userId = await _userRepository.CreateAsync(user);
            var token = _jwtService.GenerateToken(userId, user.Email);

            _logger.LogInformation("User {UserId} signed up successfully with email: {Email}", userId, request.Email);
            
            return Ok(new AuthResponse
            {
                UserId = userId,
                Token = token,
                Username = user.Username
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);
            
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Login failed: Missing email or password");
                return BadRequest("Email and password are required");
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid credentials for email: {Email}", request.Email);
                return Unauthorized("Invalid credentials");
            }

            var token = _jwtService.GenerateToken(user.Id, user.Email);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);
            
            return Ok(new AuthResponse
            {
                UserId = user.Id,
                Token = token,
                Username = user.Username
            });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
