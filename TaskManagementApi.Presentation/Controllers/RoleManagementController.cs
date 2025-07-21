using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Core.DTOs.DTO_User;
using TaskManagementApi.Core.Interface;
using TaskManagementApi.Core.Interface.IRepositories;

namespace TaskManagementApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleManagementController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        private readonly IUserRepository _userRepository;


        public RoleManagementController(IUnitOfService unitOfService, IUserRepository userRepository)
        {
            _unitOfService = unitOfService;
            _userRepository = userRepository;
        }

        // GET: api/RoleManagement/roles
        // Get all available roles in the system
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllRoles()
        {
            var roles = await _unitOfService.AuthService.GetAllRolesAsync();
            return Ok(roles);
        }

        // GET: api/RoleManagement/users/{userId}/roles
        // Get roles for a specific user
        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
        {
            // MODIFIED: Use the new GetUserRolesAsync(string userId) method from AuthService
            var roles = await _unitOfService.AuthService.GetUserRolesAsync(userId);

            return Ok(roles);
        }

        // POST: api/RoleManagement/users/{userId}/roles
        // Add a user to a specific role
        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AddUserToRole(string userId, [FromBody] DTO_UpdateUserRole roleUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _unitOfService.AuthService.AddUserToRoleAsync(userId, roleUpdateDto.RoleName);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // DELETE: api/RoleManagement/users/{userId}/roles/{roleName}
        // Remove a user from a specific role
        [HttpDelete("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            var user = await _unitOfService.AuthService.GetUserByIdAsync(userId); // MODIFIED: Use AuthService
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var (success, message) = await _unitOfService.AuthService.RemoveUserFromRoleAsync(userId, roleName);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpPost("register-with-role")]
        public async Task<IActionResult> AdminRegisterUserWithRole([FromBody] DTO_AdminRegisterUser request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _unitOfService.AuthService.AdminRegisterUserAndAssignRoleAsync(request.Email, request.Password, request.RoleName);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
    }
}
