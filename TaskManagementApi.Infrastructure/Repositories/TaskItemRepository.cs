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
            return await _context.TaskItems.ToListAsync();
        }

        public async Task<IQueryable<TaskItem>> GetAllTasksQueryable()
        {
            return _context.TaskItems.Include(t => t.User).AsQueryable();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _context.TaskItems.FindAsync(id);
        }

        public async Task AddTaskAsync(TaskItem taskItem)
        {
            _context.TaskItems.AddAsync(taskItem);

        }
        public async Task DeleteTaskAsync(TaskItem taskItem)
        {
            taskItem.IsDeleted = true;
            taskItem.DeletedAt = DateTime.UtcNow;
            _context.TaskItems.Update(taskItem);
        }

        public async Task RestoreTaskAsync(TaskItem taskItem)
        {
            taskItem.IsDeleted = false;
            taskItem.DeletedAt = null;
            _context.TaskItems.Update(taskItem);
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
