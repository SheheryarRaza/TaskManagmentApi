using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagementApi.Core.DTOs.DTO_User;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface;
using TaskManagementApi.Core.Interface.IRepositories;

namespace TaskManagementApi.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public AuthService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<(bool Success, string? Message)> ChangePasswordAsync(DTO_ChangePassowrd changePassword)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return (false, "User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(currentUser, changePassword.CurrentPassword, changePassword.NewPassword);

            if (!result.Succeeded)
            {
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            return (true, "Password changed successfully.");
        }

        public Task<User?> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId))
            {
                return Task.FromResult<User?>(null);
            }
            return _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<DTO_UserGet?> GetUserProfileAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            if(currentUser == null)
            {
                return null;
            }
            return _mapper.Map<DTO_UserGet>(currentUser);
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

        public async Task<(bool Success, string? Message)> UpdateUserProfileAsync(DTO_UpdateUser updateUser)
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser == null)
            {
                return (false, "User not found.");
            }

            if(!string.Equals(currentUser.Email, updateUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(updateUser.Email);
                if (existingUser != null && existingUser.Id != currentUser.Id)
                {
                    return (false, "Email is already taken by another user.");
                }
            }
            _mapper.Map(updateUser, currentUser);

            var result = await _userRepository.UpdateUserAsync(currentUser);

            if (!result.Succeeded)
            {
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            return (true, "Profile updated successfully.");

        }

        public async Task<IList<string>> GetCurrentUserRolesAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return new List<string>();
            }
            return await _userRepository.GetUserRolesAsync(currentUser);
        }
    }
}
