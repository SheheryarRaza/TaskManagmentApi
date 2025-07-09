using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Data;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface.IRepositories;

namespace TaskManagementApi.Core.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddTaskAsync(TaskItem taskItem)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTaskAsync(TaskItem taskItem)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TaskItemExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTaskAsync(TaskItem taskItem)
        {
            throw new NotImplementedException();
        }
    }
}
