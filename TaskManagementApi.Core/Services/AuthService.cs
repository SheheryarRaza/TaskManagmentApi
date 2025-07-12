using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface;
using TaskManagementApi.Core.Interface.IRepositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TaskManagementApi.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor )
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<User?> GetCurerntUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId))
            {
                return Task.FromResult<User?>(null);
            }
            return _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<(bool Success, string? Token, string? Message)> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !await _userRepository.CheckPasswordAsync(user, password))
            {
                return (false, null, "Invalid email or password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (true, tokenHandler.WriteToken(token), null);
        }

        public async Task<(bool Success, string? Message)> RegisterAsync(string email, string password)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if(existingUser != null)
            {
                return (false, "User with this email already exists.");
            }

            var user = new User
            {
                Email = email,
                UserName = email, // You can customize this as needed
                EmailConfirmed = false // Set to true if you want to require email confirmation
            };

            var result = await _userRepository.CreateUserAsync(user, password);
            if(!result)
            {
                return (false, "User registration failed.");
            }

            return (true, "User Registered Successfully.");
        }
    }
}
