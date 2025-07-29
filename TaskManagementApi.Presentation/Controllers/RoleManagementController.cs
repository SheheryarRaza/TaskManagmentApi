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

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllRoles()
        {
            var roles = await _unitOfService.AuthService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
        {
            var roles = await _unitOfService.AuthService.GetUserRolesAsync(userId);

            return Ok(roles);
        }

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
