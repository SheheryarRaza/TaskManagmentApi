using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Interface
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem> CreateTaskAsync(TaskItem taskItem);
        Task<bool> UpdateTaskAsync(TaskItem taskItem);
        Task<bool> DeleteTaskAsync(int id);
    }
}
