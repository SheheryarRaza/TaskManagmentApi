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
using TaskManagementApi.Core.DTOs.DTO_Tasks;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface;
using TaskManagementApi.Core.Interface.IRepositories;

namespace TaskManagementApi.Core.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public TaskItemService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper, IUserRepository userRepository)
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
        public async Task<DTO_PaginatedResult<DTO_TaskGet>> GetAllTaskAsync(TaskQueryParams queryParams)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await IsCurrentUserAdminAsync();

            IQueryable<TaskItem> query = (await _unitOfWork.TaskItemRepository.GetAllTasksQueryable());

            if (isAdmin)
            {
                query = query.Where(t => t.AssignedByUserId == currentUserId || t.UserId == currentUserId);
            }
            else
            {
                query = query.Where(t => t.UserId == currentUserId);
            }
            if (!queryParams.IncludeDeleted)
            {
                query = query.Where(t => !t.IsDeleted);
            }
            if (!queryParams.IncludeNotified)
            {
                query = query.Where(t => !t.IsNotified);
            }

            //Fitering
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                string searchLower = queryParams.Search.ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(searchLower) || (t.Description != null && t.Description.ToLower().Contains(searchLower)));
            }

            if (queryParams.IsCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == queryParams.IsCompleted.Value);
            }
            if (queryParams.DueDateFrom.HasValue)
            {
                query = query.Where(t => t.DueDate >= queryParams.DueDateFrom.Value);
            }
            if (queryParams.DueDateTo.HasValue)
            {
                query = query.Where(t => t.DueDate <= queryParams.DueDateTo.Value.AddDays(1));
            }

            int totalCount = await query.CountAsync();

            // 2. Sorting
            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                Expression<Func<TaskItem, object>> orderByExpression = queryParams.SortBy.ToLower() switch
                {
                    "title" => t => t.Title,
                    "description" => t => t.Description!,
                    "iscompleted" => t => t.IsCompleted,
                    "duedate" => t => t.DueDate!,
                    "createdat" => t => t.CreatedAt,
                    "updatedat" => t => t.UpdatedAt,
                    _ => t => t.CreatedAt // Default sort
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
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            // 3. Pagination
            var pagedTasks = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.AdjustedPageSize)
                .Take(queryParams.AdjustedPageSize)
                .ToListAsync();

            var mappedTasks = _mapper.Map<IEnumerable<DTO_TaskGet>>(pagedTasks);

            return new DTO_PaginatedResult<DTO_TaskGet>
            {
                Items = mappedTasks,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.AdjustedPageSize
            };
        }
        public async Task<DTO_TaskGet?> GetTaskByIdAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await IsCurrentUserAdminAsync();
            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);

            if (taskItem == null || (isAdmin && taskItem.AssignedByUserId != currentUserId && taskItem.UserId != currentUserId) || (!isAdmin && taskItem.UserId != currentUserId))
            {
                return null;
            }

            return _mapper.Map<DTO_TaskGet>(taskItem);
        }

        public async Task<DTO_PaginatedResult<DTO_TaskGet>> GetMyAssignedTasksAsync(TaskQueryParams queryParams, string currentUserId)
        {
            // Start with all tasks, but filter only for tasks where the current user is the assignee (UserId)
            IQueryable<TaskItem> query = await _unitOfWork.TaskItemRepository.GetAllTasksQueryable();

            query = query.Where(t => t.UserId == currentUserId);

            // Apply soft delete filter unless explicitly included
            if (!queryParams.IncludeDeleted)
            {
                query = query.Where(t => !t.IsDeleted);
            }

            // Apply notified filter unless explicitly included
            if (!queryParams.IncludeNotified)
            {
                query = query.Where(t => !t.IsNotified);
            }

            // 1. Filtering (same as GetAllTasksAsync)
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                string searchLower = queryParams.Search.ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(searchLower) ||
                                         (t.Description != null && t.Description.ToLower().Contains(searchLower)));
            }

            if (queryParams.IsCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == queryParams.IsCompleted.Value);
            }

            if (queryParams.DueDateFrom.HasValue)
            {
                query = query.Where(t => t.DueDate >= queryParams.DueDateFrom.Value);
            }

            if (queryParams.DueDateTo.HasValue)
            {
                query = query.Where(t => t.DueDate <= queryParams.DueDateTo.Value.AddDays(1));
            }

            int totalCount = await query.CountAsync();

            // 2. Sorting (same as GetAllTasksAsync)
            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                Expression<Func<TaskItem, object>> orderByExpression = queryParams.SortBy.ToLower() switch
                {
                    "title" => t => t.Title,
                    "description" => t => t.Description!,
                    "iscompleted" => t => t.IsCompleted,
                    "duedate" => t => t.DueDate!,
                    "createdat" => t => t.CreatedAt,
                    "updatedat" => t => t.UpdatedAt,
                    _ => t => t.CreatedAt
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
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            // 3. Pagination (same as GetAllTasksAsync)
            var pagedTasks = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.AdjustedPageSize)
                .Take(queryParams.AdjustedPageSize)
                .ToListAsync();

            var mappedTasks = _mapper.Map<IEnumerable<DTO_TaskGet>>(pagedTasks);

            return new DTO_PaginatedResult<DTO_TaskGet>
            {
                Items = mappedTasks,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.AdjustedPageSize
            };
        }

        public async Task<DTO_TaskGet> CreateTaskAsync(DTO_TaskPost taskPost, bool isAdmin, string currentUserId)
        {
            var taskItem = _mapper.Map<TaskItem>(taskPost);

            if (isAdmin)
            {
                if (!string.IsNullOrEmpty(taskPost.AssignedToUserId))
                {
                    var assignedUser = await _userRepository.GetUserByIdAsync(taskPost.AssignedToUserId);
                    if (assignedUser == null)
                    {
                        throw new ArgumentException("Assigned user not found.");
                    }
                    taskItem.UserId = assignedUser.Id; // Assign to the specified user
                    taskItem.AssignedByUserId = currentUserId;
                    // Admin cannot set DueDate or NotificationDateTime for assigned tasks
                    taskItem.DueDate = null;
                    taskItem.IsNotificationEnabled = false;
                    taskItem.NotificationDateTime = null;
                }
                else
                {
                    taskItem.UserId = currentUserId; // Assign to the current user
                    taskItem.AssignedByUserId = currentUserId;
                }
            }
            else
            {
                // Regular user creates a task for themselves
                taskItem.UserId = currentUserId; // Assignee is the user
                taskItem.AssignedByUserId = null; // Not assigned by an admin
            }


            taskItem.CreatedAt = DateTime.UtcNow;
            taskItem.UpdatedAt = DateTime.UtcNow;
            taskItem.IsCompleted = false;
            taskItem.IsDeleted = false;
            taskItem.DeletedAt = null;
            taskItem.IsNotified = false;

            await _unitOfWork.TaskItemRepository.AddTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DTO_TaskGet>(taskItem);
        }
        public async Task<bool> UpdateTaskAsync(int id, DTO_TaskPut taskPut, bool isAdmin, string currentUserId)
        {
            var existingTask = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);

            // Admin can update any task, regular users can only update their own
            if (existingTask == null || existingTask.IsDeleted) // Cannot update a soft-deleted task
            {
                return false;
            }

            if (isAdmin)
            {
                if (existingTask.AssignedByUserId != currentUserId && existingTask.UserId != currentUserId)
                {
                    return false; // Admin cannot update tasks they didn't assign and are not assigned to them
                }
            }
            else // Regular user
            {
                if (existingTask.UserId != currentUserId)
                {
                    return false; // Regular user can only update their own tasks
                }
            }

            // Admin reassignment logic
            if (isAdmin && !string.IsNullOrEmpty(taskPut.AssignedToUserId) && existingTask.UserId != taskPut.AssignedToUserId)
            {
                var newAssignedUser = await _userRepository.GetUserByIdAsync(taskPut.AssignedToUserId);
                if (newAssignedUser == null)
                {
                    // If the assigned user ID is invalid, prevent the update or return an error
                    return false; // Or throw new ArgumentException("New assigned user not found.");
                }
                existingTask.UserId = newAssignedUser.Id;
                existingTask.AssignedByUserId = currentUserId;
                existingTask.DueDate = null;
                existingTask.IsNotificationEnabled = false;
                existingTask.NotificationDateTime = null;
                existingTask.IsNotified = false; // Reset notification status for the new assignee
            }
            else
            {
                _mapper.Map(taskPut, existingTask);

                if (!isAdmin || existingTask.UserId == currentUserId)
                {
                    existingTask.DueDate = taskPut.DueDate;
                    existingTask.IsNotificationEnabled = taskPut.IsNotificationEnabled;
                    existingTask.NotificationDateTime = taskPut.NotificationDateTime;
                }

                if (existingTask.IsNotificationEnabled && existingTask.IsNotified &&
                    (taskPut.IsNotificationEnabled != existingTask.IsNotificationEnabled ||
                     taskPut.NotificationDateTime != existingTask.NotificationDateTime))
                {
                    existingTask.IsNotified = false;
                }
            }

            existingTask.UpdatedAt = DateTime.UtcNow; // Update timestamp

            await _unitOfWork.TaskItemRepository.UpdateTaskAsync(existingTask);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await IsCurrentUserAdminAsync();

            if (!isAdmin)
            {
                return false; // Unauthorized
            }

            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);

            if (taskItem == null || taskItem.IsDeleted) // Cannot delete an already soft-deleted task
            {
                return false;
            }
            await _unitOfWork.TaskItemRepository.DeleteTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RestoreTaskAsync(int id)
        {
            var currentUser = GetCurrentUserId();
            var isAdmin = await IsCurrentUserAdminAsync();

            if (!isAdmin)
            {
                return false; // Unauthorized
            }

            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);
            if (taskItem == null || !taskItem.IsDeleted) // Can only restore if it exists and is soft-deleted
            {
                return false;
            }

            await _unitOfWork.TaskItemRepository.RestoreTaskAsync(taskItem);
            taskItem.IsNotified = false;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> MarkTaskAsNotifiedAsync(int id) // NEW: For background service to mark as notified
        {
            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);

            if (taskItem == null || taskItem.IsDeleted || taskItem.IsCompleted || taskItem.IsNotified)
            {
                return false;
            }

            taskItem.IsNotified = true;
            await _unitOfWork.TaskItemRepository.UpdateTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<TaskItem>> GetTasksForNotificationAsync(TimeSpan notificationLeadTime)
        {
            var now = DateTime.UtcNow;
            var notificationWindowStart = now.Add(notificationLeadTime);

            var tasksQueryable = await _unitOfWork.TaskItemRepository.GetAllTasksQueryable();

            return tasksQueryable
                .Where(t => t.IsNotificationEnabled &&
                            !t.IsNotified && // Only tasks not yet notified
                            !t.IsCompleted && // Only incomplete tasks
                            t.NotificationDateTime.HasValue &&
                            t.NotificationDateTime.Value <= notificationWindowStart &&
                            t.NotificationDateTime.Value >= now) // Within the notification window
                .ToList();
        }
    }
}
