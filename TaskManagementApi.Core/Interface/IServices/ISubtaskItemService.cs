using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.DTOs.DTO_Subtask;
using TaskManagementApi.Core.DTOs.DTO_Tasks;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Interface.IServices
{
    public interface ISubtaskItemService
    {
        Task<DTO_PaginatedResult<DTO_SubtaskGet>> GetAllSubTasksAsync(int parentTaskId, SubTaskQueryParams queryParams);
        Task<DTO_SubtaskGet?> GetSubTaskByIdAsync(int id);
        Task<SubTaskItem> CreateSubTaskAsync(int parentTaskId, DTO_SubtaskPost subTaskPost, string currentUserId);
        Task<bool> UpdateSubTaskAsync(int id, DTO_SubtaskPut subTaskPut, string currentUserId);
        Task<bool> DeleteSubTaskAsync(int id, string currentUserId); 

        Task<bool> RestoreSubTaskAsync(int id, string currentUserId);
    }
}
