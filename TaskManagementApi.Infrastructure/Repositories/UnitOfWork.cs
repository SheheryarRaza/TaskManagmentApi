using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Data;
using TaskManagementApi.Core.Interface.IRepositories;

namespace TaskManagementApi.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITaskItemRepository _taskItemRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public ITaskItemRepository TaskItemRepository
        {
            get
            {
                _taskItemRepository ??= new TaskItemRepository(_context);
                return _taskItemRepository;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
