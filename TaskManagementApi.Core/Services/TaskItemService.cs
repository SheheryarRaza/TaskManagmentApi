using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface.IRepositories;
using TaskManagementApi.Core.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskManagementApi.Core.DTOs;
using AutoMapper;

namespace TaskManagementApi.Core.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TaskItemService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        private string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID not found.");
            }
            return userId;
        }
        public async Task<DTO_TaskGet> CreateTaskAsync(DTO_TaskPost taskPost)
        {
            var taskItem = _mapper.Map<TaskItem>(taskPost);
            taskItem.UserId = GetCurrentUserId();
            taskItem.CreatedAt = DateTime.UtcNow;
            taskItem.UpdatedAt = DateTime.UtcNow;
            taskItem.IsCompleted = false;

            await _unitOfWork.TaskItemRepository.AddTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DTO_TaskGet>(taskItem);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);
            if (taskItem == null || taskItem.UserId != currentUserId)
            {
                return false;
            }
            await _unitOfWork.TaskItemRepository.DeleteTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DTO_TaskGet>> GetAllTasksAsync()
        {
            var currentUserId = GetCurrentUserId();
            var tasks = (await _unitOfWork.TaskItemRepository.GetAllTasksAsync())
                        .Where(t => t.UserId == currentUserId);

            return _mapper.Map<IEnumerable<DTO_TaskGet>>(tasks);

        }

        public async Task<DTO_TaskGet?> GetTaskByIdAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);

            if (taskItem == null || taskItem.UserId != currentUserId)
            {
                return null;
            }

            return _mapper.Map<DTO_TaskGet>(taskItem);
        }

        public async Task<bool> UpdateTaskAsync(int id , DTO_TaskPut taskPut)
        {
            var currentUserId = GetCurrentUserId();
            var existingTask = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);

            if (existingTask == null || existingTask.UserId != currentUserId)
            {
                return false;
            }

            existingTask.Title = taskPut.Title;
            existingTask.Description = taskPut.Description;
            existingTask.IsCompleted = taskPut.IsCompleted;
            existingTask.DueDate = taskPut.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TaskItemRepository.UpdateTaskAsync(existingTask);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
