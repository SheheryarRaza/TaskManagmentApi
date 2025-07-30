using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Interface.IRepositories
{
    public interface ISubtaskItemRepository
    {
        IQueryable<SubTaskItem> GetAllSubTasksQueryable();
        Task<SubTaskItem?> GetSubTaskByIdAsync(int id);
        Task AddSubTaskAsync(SubTaskItem subTaskItem);
        Task UpdateSubTaskAsync(SubTaskItem subTaskItem);
        Task DeleteSubTaskAsync(SubTaskItem subTaskItem);
        Task RestoreSubTaskAsync(SubTaskItem subTaskItem);
        Task<bool> SubTaskItemExistsAsync(int id);
    }
}
