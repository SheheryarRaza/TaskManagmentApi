using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Core.DTOs.DTO_Tasks;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TaskController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        public TaskController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        private string GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims.");
            }
            return userId;
        }

        private async Task<bool> IsCurrentUserAdminAsync()
        {
            var roles = await _unitOfService.AuthService.GetCurrentUserRolesAsync();
            return roles.Contains("Admin");
        }

        [HttpGet]
        public async Task<ActionResult<DTO_PaginatedResult<DTO_TaskGet>>> GetTaskItems([FromQuery] TaskQueryParams queryParams)
        {
            var paginatedTasks = await _unitOfService.TaskItemService.GetAllTaskAsync(queryParams);
            return Ok(paginatedTasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var taskItem = await _unitOfService.TaskItemService.GetTaskByIdAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return Ok(taskItem);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTaskItem(DTO_TaskPost taskPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = GetCurrentUserId();
            var isAdmin = await IsCurrentUserAdminAsync();

            try
            {
                var createdTask = await _unitOfService.TaskItemService.CreateTaskAsync(taskPost, isAdmin, currentUserId);
                // Return TaskGetDto after creation if desired, or the entity itself
                return CreatedAtAction(nameof(GetTaskItem), new { id = createdTask.Id }, createdTask);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutTaskItem(int id,[FromBody] DTO_TaskPut taskPut)
        {
            if (id != taskPut.Id) // Ensure ID in route matches ID in body
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = GetCurrentUserId();
            var isAdmin = await IsCurrentUserAdminAsync();

            var result = await _unitOfService.TaskItemService.UpdateTaskAsync(id, taskPut, isAdmin, currentUserId);

            if (!result)
            {
                return NotFound("Task not found or you do not have permission to update it.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TaskItem>> DeleteTaskItem(int id)
        {
            var result = await _unitOfService.TaskItemService.DeleteTaskAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreTaskItem(int id)
        {
            var result = await _unitOfService.TaskItemService.RestoreTaskAsync(id);

            if (!result)
            {
                return NotFound("Task not found or not soft-deleted, or you don't have permission.");
            }

            return Ok(new { message = $"Task with ID {id} restored successfully." });
        }
    }

}
