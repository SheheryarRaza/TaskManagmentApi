using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.DTOs.DTO_User;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Interface
{
    public interface IAuthService
    {
        Task<(bool Success, string? Token, string? Message)> LoginAsync(string email, string password);

        Task<(bool Success, string? Message)> RegisterAsync (string email, string password);

        Task<User?> GetCurrentUserAsync();

        Task<DTO_GetUser?> GetUserProfileAsync();

        Task<IList<string>> GetCurrentUserRolesAsync();

        Task<(bool Success, string? Message)> UpdateUserProfileAsync(DTO_UpdateUser updateUser);

        Task<(bool Success, string? Message)> ChangePasswordAsync(DTO_ChangePassowrd changePassword);
        Task<IList<string>> GetAllRolesAsync();

        Task<User?> GetUserByIdAsync(string userId);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<(bool Success, string? Message)> AddUserToRoleAsync(string userId, string roleName);
        Task<(bool Success, string? Message)> RemoveUserFromRoleAsync(string userId, string roleName);

        // NEW: Admin functionality to create a new role
        Task<(bool Success, string? Message)> CreateRoleAsync(string roleName);

        // NEW: Admin functionality to register a user and assign a role
        Task<(bool Success, string? Message)> AdminRegisterUserAndAssignRoleAsync(string email, string password, string roleName);


    }
}
