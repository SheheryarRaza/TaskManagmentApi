using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Core.DTOs.DTO_Subtask;
using TaskManagementApi.Core.DTOs.DTO_Tasks;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Presentation.Controllers
{
    [Route("api/Tasks/{parentTaskId}/[controller]")] // Nested route for subtasks
    [ApiController]
    [Authorize]
    public class SubtaskItemController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        private readonly IMapper _mapper;

        public SubtaskItemController(IUnitOfService unitOfService, IMapper mapper)
        {
            _unitOfService = unitOfService;
            _mapper = mapper;
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

        [HttpGet]
        public async Task<ActionResult<DTO_PaginatedResult<DTO_SubtaskGet>>> GetSubTaskItems(int parentTaskId, [FromQuery] SubTaskQueryParams queryParams)
        {
            var paginatedSubTasks = await _unitOfService.SubtaskItemService.GetAllSubTasksAsync(parentTaskId, queryParams);
            return Ok(paginatedSubTasks);
        }

        // GET: api/Tasks/{parentTaskId}/SubTasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_SubtaskGet>> GetSubTaskItem(int id)
        {
            var subTaskItem = await _unitOfService.SubtaskItemService.GetSubTaskByIdAsync(id);

            if (subTaskItem == null)
            {
                return NotFound();
            }

            return Ok(subTaskItem);
        }

        // POST: api/Tasks/{parentTaskId}/SubTasks
        [HttpPost]
        public async Task<ActionResult<SubTaskItem>> PostSubTaskItem(int parentTaskId, [FromBody] DTO_SubtaskPost subTaskPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = GetCurrentUserId();

            try
            {
                var createdSubTask = await _unitOfService.SubtaskItemService.CreateSubTaskAsync(parentTaskId, subTaskPost, currentUserId);
                // Map the created entity to the DTO before returning
                var createdSubTaskDto = _mapper.Map<DTO_SubtaskGet>(createdSubTask); // NEW: Map to DTO
                return CreatedAtAction(nameof(GetSubTaskItem), new { parentTaskId = createdSubTaskDto.ParentTaskId, id = createdSubTaskDto.Id }, createdSubTaskDto); // MODIFIED to use DTO
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Tasks/{parentTaskId}/SubTasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubTaskItem(int id, [FromBody] DTO_SubtaskPut subTaskPut)
        {
            if (id != subTaskPut.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = GetCurrentUserId();
            var result = await _unitOfService.SubtaskItemService.UpdateSubTaskAsync(id, subTaskPut, currentUserId);

            if (!result)
            {
                return NotFound("Subtask not found or you do not have permission to update it.");
            }

            return NoContent();
        }

        // DELETE: api/Tasks/{parentTaskId}/SubTasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubTaskItem(int id)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _unitOfService.SubtaskItemService.DeleteSubTaskAsync(id, currentUserId);

            if (!result)
            {
                return NotFound("Subtask not found, already deleted, or you do not have permission to delete it.");
            }

            return NoContent();
        }

        // POST: api/Tasks/{parentTaskId}/SubTasks/{id}/restore
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreSubTaskItem(int id)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _unitOfService.SubtaskItemService.RestoreSubTaskAsync(id, currentUserId);

            if (!result)
            {
                return NotFound("Subtask not found, not soft-deleted, or you do not have permission to restore it.");
            }

            return Ok(new { message = $"Subtask with ID {id} restored successfully." });
        }

    }
}
