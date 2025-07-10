using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TaskController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        public TaskController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems()
        {
            var tasks = await _unitOfService.TaskItemService.GetAllTasksAsync();
            return Ok(tasks);
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
        public async Task<ActionResult<TaskItem>> PostTaskItem(TaskItem taskItem)
        {
            var createdItem = await _unitOfService.TaskItemService.CreateTaskAsync(taskItem);
            return CreatedAtAction(nameof(GetTaskItems), new { id = createdItem.Id }, createdItem);
        }

        [HttpPut]
        public async Task<IActionResult> PutTaskItem(TaskItem taskItem)
        {
            var result = await _unitOfService.TaskItemService.UpdateTaskAsync(taskItem);

            if (!result)
            {
                return NotFound();
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

    }

}
