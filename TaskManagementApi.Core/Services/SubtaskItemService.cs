using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Core.DTOs.DTO_Subtask;
using TaskManagementApi.Core.DTOs.DTO_Tasks;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface.IRepositories;
using TaskManagementApi.Core.Interface.IServices;

namespace TaskManagementApi.Core.Services
{
    public class SubtaskItemService : ISubtaskItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public SubtaskItemService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        private string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID not found.");
            }
            return userId;
        }

        private async Task<bool> IsCurrentUserAdminAsync()
        {
            var currentUser = await _userRepository.GetUserByIdAsync(GetCurrentUserId());
            if (currentUser == null) return false;
            var roles = await _userRepository.GetUserRolesAsync(currentUser);
            return roles.Contains("Admin");
        }

        public async Task<DTO_PaginatedResult<DTO_SubtaskGet>> GetAllSubTasksAsync(int parentTaskId, SubTaskQueryParams queryParams)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await IsCurrentUserAdminAsync();

            IQueryable<SubTaskItem> query = _unitOfWork.SubtaskItemRepository.GetAllSubTasksQueryable();

            // Always filter by parent task
            query = query.Where(st => st.ParentTaskId == parentTaskId);

            // If not admin, filter by current user for subtasks
            if (!isAdmin)
            {
                query = query.Where(st => st.UserId == currentUserId);
            }

            // Apply soft delete filter unless explicitly included
            if (!queryParams.IncludeDeleted)
            {
                query = query.Where(st => !st.IsDeleted);
            }

            // Filtering
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                string searchLower = queryParams.Search.ToLower();
                query = query.Where(st => st.Title.ToLower().Contains(searchLower) ||
                                         (st.Description != null && st.Description.ToLower().Contains(searchLower)));
            }

            if (queryParams.IsCompleted.HasValue)
            {
                query = query.Where(st => st.IsCompleted == queryParams.IsCompleted.Value);
            }

            if (queryParams.DueDateFrom.HasValue)
            {
                query = query.Where(st => st.DueDate >= queryParams.DueDateFrom.Value);
            }

            if (queryParams.DueDateTo.HasValue)
            {
                query = query.Where(st => st.DueDate <= queryParams.DueDateTo.Value.AddDays(1));
            }
            if (queryParams.Priority.HasValue)
            {
                query = query.Where(st => st.Priority == queryParams.Priority.Value);
            }

            int totalCount = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                Expression<Func<SubTaskItem, object>> orderByExpression = queryParams.SortBy.ToLower() switch
                {
                    "title" => st => st.Title,
                    "description" => st => st.Description!,
                    "iscompleted" => st => st.IsCompleted,
                    "duedate" => st => st.DueDate!,
                    "createdat" => st => st.CreatedAt,
                    "updatedat" => st => st.UpdatedAt,
                    "priority" => st => st.Priority,
                    _ => st => st.CreatedAt
                };

                if (queryParams.SortOrder?.ToLower() == "desc")
                {
                    query = query.OrderByDescending(orderByExpression);
                }
                else
                {
                    query = query.OrderBy(orderByExpression);
                }
            }
            else
            {
                query = query.OrderByDescending(st => st.CreatedAt);
            }

            // Pagination
            var pagedSubTasks = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.AdjustedPageSize)
                .Take(queryParams.AdjustedPageSize)
                .ToListAsync();

            var mappedSubTasks = _mapper.Map<IEnumerable<DTO_SubtaskGet>>(pagedSubTasks);

            return new DTO_PaginatedResult<DTO_SubtaskGet>
            {
                Items = mappedSubTasks,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.AdjustedPageSize
            };
        }

        public async Task<DTO_SubtaskGet?> GetSubTaskByIdAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await IsCurrentUserAdminAsync();

            var subTaskItem = await _unitOfWork.SubtaskItemRepository.GetSubTaskByIdAsync(id);

            // Admin can view any subtask, regular users can only view their own
            if (subTaskItem == null || (!isAdmin && subTaskItem.UserId != currentUserId))
            {
                return null;
            }

            return _mapper.Map<DTO_SubtaskGet>(subTaskItem);
        }

        public async Task<SubTaskItem> CreateSubTaskAsync(int parentTaskId, DTO_SubtaskPost subTaskPost, string currentUserId)
        {
            // Verify parent task exists and belongs to the current user (or admin)
            var parentTask = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(parentTaskId);
            if (parentTask == null || parentTask.UserId != currentUserId)
            {
                throw new ArgumentException("Parent task not found or does not belong to the current user.");
            }

            var subTaskItem = _mapper.Map<SubTaskItem>(subTaskPost);

            subTaskItem.ParentTaskId = parentTaskId;
            subTaskItem.UserId = currentUserId; // Subtask is owned by the user who creates it
            subTaskItem.CreatedAt = DateTime.UtcNow;
            subTaskItem.UpdatedAt = DateTime.UtcNow;
            subTaskItem.IsCompleted = false;
            subTaskItem.IsDeleted = false;
            subTaskItem.DeletedAt = null;

            await _unitOfWork.SubtaskItemRepository.AddSubTaskAsync(subTaskItem);
            await _unitOfWork.SaveChangesAsync();
            return subTaskItem;
        }

        public async Task<bool> UpdateSubTaskAsync(int id, DTO_SubtaskPut subTaskPut, string currentUserId)
        {
            var existingSubTask = await _unitOfWork.SubtaskItemRepository.GetSubTaskByIdAsync(id);

            if (existingSubTask == null || existingSubTask.UserId != currentUserId || existingSubTask.IsDeleted)
            {
                return false;
            }

            _mapper.Map(subTaskPut, existingSubTask);
            existingSubTask.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SubtaskItemRepository.UpdateSubTaskAsync(existingSubTask);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSubTaskAsync(int id, string currentUserId)
        {
            var subTaskItem = await _unitOfWork.SubtaskItemRepository.GetSubTaskByIdAsync(id);

            if (subTaskItem == null || subTaskItem.UserId != currentUserId || subTaskItem.IsDeleted)
            {
                return false;
            }

            await _unitOfWork.SubtaskItemRepository.DeleteSubTaskAsync(subTaskItem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreSubTaskAsync(int id, string currentUserId)
        {
            var subTaskItem = await _unitOfWork.SubtaskItemRepository.GetSubTaskByIdAsync(id);

            if (subTaskItem == null || subTaskItem.UserId != currentUserId || !subTaskItem.IsDeleted)
            {
                return false;
            }

            await _unitOfWork.SubtaskItemRepository.RestoreSubTaskAsync(subTaskItem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
