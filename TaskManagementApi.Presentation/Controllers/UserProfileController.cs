using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskManagementApi.Core.DTOs.DTO_User;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        public UserProfileController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        // GET: api/UserProfile
        [HttpGet]
        public async Task<ActionResult<DTO_UserGet>> GetUserProfile()
        {
            var userProfile = await _unitOfService.AuthService.GetUserProfileAsync();
            if (userProfile == null)
            {
                return NotFound("User profile not found.");
            }
            return Ok(userProfile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] DTO_UpdateUser UpdateUser)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _unitOfService.AuthService.UpdateUserProfileAsync(UpdateUser);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpPost("change-password")]

        public async Task<IActionResult> ChangePassword([FromBody] DTO_ChangePassowrd changePassword)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var (success, message) = await _unitOfService.AuthService.ChangePasswordAsync(changePassword);
            if (!success)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message });
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IList<string>>> GetUserRoles()
        {
            var roles = await _unitOfService.AuthService.GetCurrentUserRolesAsync();
            return Ok(roles);
        }

    }
}
