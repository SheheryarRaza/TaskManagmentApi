using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Core.Data;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface.IRepositories;

namespace TaskManagementApi.Infrastructure.Repositories
{
    public class SubtaskItemRepository : ISubtaskItemRepository
    {
        private readonly ApplicationDbContext _context;

        public SubtaskItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<SubTaskItem> GetAllSubTasksQueryable()
        {
            return _context.SubTaskItems.Include(st => st.User).AsQueryable();
        }

        public async Task<SubTaskItem?> GetSubTaskByIdAsync(int id)
        {
            return await _context.SubTaskItems.Include(st => st.User).IgnoreQueryFilters().FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task AddSubTaskAsync(SubTaskItem subTaskItem)
        {
            _context.SubTaskItems.Add(subTaskItem);
        }

        public async Task UpdateSubTaskAsync(SubTaskItem subTaskItem)
        {
            _context.Entry(subTaskItem).State = EntityState.Modified;
        }

        public async Task DeleteSubTaskAsync(SubTaskItem subTaskItem)
        {
            subTaskItem.IsDeleted = true;
            subTaskItem.DeletedAt = DateTime.UtcNow;
            _context.Entry(subTaskItem).State = EntityState.Modified;
        }

        public async Task RestoreSubTaskAsync(SubTaskItem subTaskItem)
        {
            subTaskItem.IsDeleted = false;
            subTaskItem.DeletedAt = null;
            _context.Entry(subTaskItem).State = EntityState.Modified;
        }

        public async Task<bool> SubTaskItemExistsAsync(int id)
        {
            return await _context.SubTaskItems.IgnoreQueryFilters().AnyAsync(e => e.Id == id);
        }
    }
}
