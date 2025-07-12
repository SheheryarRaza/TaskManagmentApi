using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _context.TaskItems.Include(t => t.User).ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _context.TaskItems.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddTaskAsync(TaskItem taskItem)
        {
            _context.TaskItems.AddAsync(taskItem);

        }
        public async Task DeleteTaskAsync(TaskItem taskItem)
        {
            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TaskItemExistsAsync(int id)
        {
            return await _context.TaskItems.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> UpdateTaskAsync(TaskItem taskItem)
        {
            _context.TaskItems.Update(taskItem);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
