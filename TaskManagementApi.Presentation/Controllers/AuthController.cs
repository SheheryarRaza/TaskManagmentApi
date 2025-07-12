using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public AuthController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        public class RegisterRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            public string Password { get; set; } = string.Empty;
        }

        public class LoginRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _unitOfService.AuthService.RegisterAsync(request.Email, request.Password);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, token, message) = await _unitOfService.AuthService.LoginAsync(request.Email, request.Password);

            if (!success || string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message });
            }

            return Ok(new { token });
        }


    }
}
