using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Core.DTOs.DTO_Tag;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface.IRepositories;
using TaskManagementApi.Core.Interface.IServices;

namespace TaskManagementApi.Core.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TagService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DTO_Tag>> GetAllTagsAsync()
        {
            var tags = await _unitOfWork.TagRepository.GetAllTagsQueryable().ToListAsync();
            return _mapper.Map<IEnumerable<DTO_Tag>>(tags);
        }

        public async Task<DTO_Tag?> GetTagByIdAsync(int id)
        {
            var tag = await _unitOfWork.TagRepository.GetTagByIdAsync(id);
            return _mapper.Map<DTO_Tag>(tag);
        }

        public async Task<DTO_Tag> CreateTagAsync(DTO_Tag tagDto)
        {
            var existingTag = await _unitOfWork.TagRepository.GetTagByNameAsync(tagDto.Name);
            if (existingTag != null)
            {
                throw new ArgumentException($"Tag with name '{tagDto.Name}' already exists.");
            }

            var tag = _mapper.Map<Tag>(tagDto);
            await _unitOfWork.TagRepository.AddTagAsync(tag);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DTO_Tag>(tag);
        }

        public async Task<bool> UpdateTagAsync(int id, DTO_Tag tagDto)
        {
            var existingTag = await _unitOfWork.TagRepository.GetTagByIdAsync(id);
            if (existingTag == null)
            {
                return false;
            }

            // Check if new name conflicts with existing tag
            if (!string.Equals(existingTag.Name, tagDto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var tagWithSameName = await _unitOfWork.TagRepository.GetTagByNameAsync(tagDto.Name);
                if (tagWithSameName != null && tagWithSameName.Id != id)
                {
                    throw new ArgumentException($"Tag with name '{tagDto.Name}' already exists.");
                }
            }

            _mapper.Map(tagDto, existingTag);
            await _unitOfWork.TagRepository.UpdateTagAsync(existingTag);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _unitOfWork.TagRepository.GetTagByIdAsync(id);
            if (tag == null)
            {
                return false;
            }

            // Check if any tasks are associated with this tag before deleting
            var tasksWithTag = await _unitOfWork.TaskItemRepository.GetAllTasksQueryable();

            tasksWithTag = tasksWithTag.Where(t => t.TaskItemTags.Any(tit => tit.TagId == id));

            if (tasksWithTag != null)
            {
                throw new InvalidOperationException($"Tag '{tag.Name}' cannot be deleted because it is associated with one or more tasks.");
            }

            await _unitOfWork.TagRepository.DeleteTagAsync(tag);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
