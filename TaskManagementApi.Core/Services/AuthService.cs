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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
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

        public async Task<DTO_GetUser?> GetUserProfileAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            if(currentUser == null)
            {
                return null;
            }
            return _mapper.Map<DTO_GetUser>(currentUser);
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
            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation($"Login: User '{user.UserName}' (ID: {user.Id}) retrieved roles: {string.Join(", ", roles)}");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (true, tokenHandler.WriteToken(token), "Login successful.");
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
                UserName = email,
                EmailConfirmed = false
            };

            var result = await _userRepository.CreateUserAsync(user, password);
            if(!result)
            {
                return (false, "User registration failed.");
            }

            await _userManager.AddToRoleAsync(user, "User");

            return (true, "User registered successfully.");
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

        public async Task<IList<string>> GetCurrentUserRolesAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return new List<string>();
            }
            return await _userRepository.GetUserRolesAsync(currentUser);
        }

        public async Task<IList<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        }

        public async Task<(bool Success, string? Message)> AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return (false, $"Role '{roleName}' does not exist.");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            return (true, $"User '{user.UserName}' successfully added to role '{roleName}'.");
        }

        public async Task<(bool Success, string? Message)> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                return (false, $"User '{user.UserName}' is not in role '{roleName}'.");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            return (true, $"User '{user.UserName}' successfully removed from role '{roleName}'.");
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }
        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new List<string>();
            }
            return await _userRepository.GetUserRolesAsync(user);
        }

        public async Task<(bool Success, string? Message)> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return (false, $"Role '{roleName}' already exists.");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            return (true, $"Role '{roleName}' created successfully.");
        }

        public async Task<(bool Success, string? Message)> AdminRegisterUserAndAssignRoleAsync(string email, string password, string roleName)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return (false, "User with this email already exists.");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return (false, $"Role '{roleName}' does not exist.");
            }

            var user = new User { UserName = email, Email = email };
            var createResult = await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                return (false, string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }

            var assignRoleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!assignRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return (false, $"User created but failed to assign role: {string.Join("; ", assignRoleResult.Errors.Select(e => e.Description))}");
            }

            return (true, $"User '{user.UserName}' registered and assigned to role '{roleName}' successfully.");
        }

    }
}
