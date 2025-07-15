using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Interface.IRepositories
{
    public interface ITaskItemRepository
    {
        Task<IQueryable<TaskItem>> GetAllTasksQueryable();
        public Task<IEnumerable<TaskItem>> GetAllTasksAsync();

        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task AddTaskAsync(TaskItem taskItem);
        Task<bool> UpdateTaskAsync(TaskItem taskItem);
        Task DeleteTaskAsync(TaskItem taskItem);
        Task<bool> TaskItemExistsAsync(int id);

        Task RestoreTaskAsync(TaskItem taskItem);
    }
}
