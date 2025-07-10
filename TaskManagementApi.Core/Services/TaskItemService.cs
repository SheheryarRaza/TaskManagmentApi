using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface.IRepositories;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Core.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<TaskItem> CreateTaskAsync(TaskItem taskItem)
        {
            taskItem.CreatedAt = DateTime.UtcNow;
            taskItem.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TaskItemRepository.AddTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();

            return taskItem;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);
            if(taskItem == null)
            {
                return false; // Task not found
            }
            await _unitOfWork.TaskItemRepository.DeleteTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
           return await _unitOfWork.TaskItemRepository.GetAllTasksAsync();

        }

        public Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);
        }

        public async Task<bool> UpdateTaskAsync(TaskItem taskItem)
        {
            var existingTask = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(taskItem.Id);

            if (existingTask == null)
            {
                return false; // Task not found
            }


            existingTask.Title = taskItem.Title;
            existingTask.Description = taskItem.Description;
            existingTask.IsCompleted = taskItem.IsCompleted;
            existingTask.DueDate = taskItem.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TaskItemRepository.UpdateTaskAsync(existingTask);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
