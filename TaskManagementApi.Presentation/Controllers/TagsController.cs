using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Core.DTOs.DTO_Tag;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public TagsController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Tag>>> GetAllTags()
        {
            var tags = await _unitOfService.TagService.GetAllTagsAsync();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_Tag>> GetTag(int id)
        {
            var tag = await _unitOfService.TagService.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DTO_Tag>> CreateTag([FromBody] DTO_Tag tagDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var createdTag = await _unitOfService.TagService.CreateTagAsync(tagDto);
                return CreatedAtAction(nameof(GetTag), new { id = createdTag.Id }, createdTag);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] DTO_Tag tagDto)
        {
            if (id != tagDto.Id)
            {
                return BadRequest("Tag ID in route does not match ID in body.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _unitOfService.TagService.UpdateTagAsync(id, tagDto);
                if (!result)
                {
                    return NotFound("Tag not found.");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var result = await _unitOfService.TagService.DeleteTagAsync(id);
                if (!result)
                {
                    return NotFound("Tag not found.");
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
