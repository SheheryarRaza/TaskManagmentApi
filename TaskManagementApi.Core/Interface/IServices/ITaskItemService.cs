using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.DTOs.DTO_Tasks;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Interface
{
    public interface ITaskItemService
    {
        Task<DTO_PaginatedResult<DTO_TaskGet>> GetAllTaskAsync(TaskQueryParams queryParams);
        Task<DTO_TaskGet?> GetTaskByIdAsync(int id);
        Task<DTO_PaginatedResult<DTO_TaskGet>> GetMyAssignedTasksAsync(TaskQueryParams queryParams, string currentUserId);
        Task<DTO_TaskGet> CreateTaskAsync(DTO_TaskPost taskPost, bool isAdmin, string currentUserId);
        Task<bool> UpdateTaskAsync(int id , DTO_TaskPut taskPut, bool isAdmin, string currentUserId);
        Task<bool> DeleteTaskAsync(int id);
        Task<bool> RestoreTaskAsync(int id);
        Task<bool> MarkTaskAsNotifiedAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksForNotificationAsync(TimeSpan notificationLeadTime);
    }
}
