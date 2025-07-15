using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Interface.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<string?> GenerateEmailConfirmationTokenAsync(User user);
        Task<bool> ConfirmEmailAsync(User user, string token);
        Task<IList<string>> GetRolesAsync(User user);

        Task<IdentityResult> UpdateUserAsync(User user);
    }
}
