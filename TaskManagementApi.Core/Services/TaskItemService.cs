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
        public Task<TaskItem> CreateTaskAsync(TaskItem taskItem)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTaskAsync(int id)
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

        public Task<bool> UpdateTaskAsync(TaskItem taskItem)
        {
            throw new NotImplementedException();
        }
    }
}
