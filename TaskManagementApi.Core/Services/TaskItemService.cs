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
using TaskManagementApi.Core.DTOs;
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

        public TaskItemService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
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
        public async Task<DTO_PaginatedResult<DTO_TaskGet>> GetAllTaskAsync(TaskQueryParams queryParams)
        {
            var currentUserId = GetCurrentUserId();
            IQueryable<TaskItem> query = (await _unitOfWork.TaskItemRepository.GetAllTasksQueryable())
                                        .Where(t => t.UserId == currentUserId);

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
                // Add one day to include tasks due on the exact DueDateTo
                query = query.Where(t => t.DueDate <= queryParams.DueDateTo.Value.AddDays(1));
            }

            int totalCount = await query.CountAsync();

            // 2. Sorting
            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                var parameter = Expression.Parameter(typeof(TaskItem), "t");
                var property = Expression.Property(parameter, queryParams.SortBy);
                var lambda = Expression.Lambda(property, parameter);

                if (queryParams.SortOrder?.ToLower() == "desc")
                {
                    query = Queryable.OrderByDescending(query, (dynamic)lambda);
                }
                else
                {
                    query = Queryable.OrderBy(query, (dynamic)lambda);
                }
            }
            else
            {
                // Default sorting if no SortBy is provided
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            // 3. Pagination
            var pagedTasks = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.AdjustedPageSize)
                .Take(queryParams.AdjustedPageSize)
                .ToListAsync();

            // Map TaskItem entities to TaskGetDto
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
            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);

            if (taskItem == null || taskItem.UserId != currentUserId)
            {
                return null;
            }

            return _mapper.Map<DTO_TaskGet>(taskItem);
        }
        public async Task<DTO_TaskGet> CreateTaskAsync(DTO_TaskPost taskPost)
        {
            var taskItem = _mapper.Map<TaskItem>(taskPost);
            taskItem.UserId = GetCurrentUserId();
            taskItem.CreatedAt = DateTime.UtcNow;
            taskItem.UpdatedAt = DateTime.UtcNow;
            taskItem.IsCompleted = false;

            await _unitOfWork.TaskItemRepository.AddTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DTO_TaskGet>(taskItem);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var taskItem = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);
            if (taskItem == null || taskItem.UserId != currentUserId)
            {
                return false;
            }
            await _unitOfWork.TaskItemRepository.DeleteTaskAsync(taskItem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTaskAsync(int id , DTO_TaskPut taskPut)
        {
            var currentUserId = GetCurrentUserId();
            var existingTask = await _unitOfWork.TaskItemRepository.GetTaskByIdAsync(id);

            if (existingTask == null || existingTask.UserId != currentUserId)
            {
                return false;
            }

            existingTask.Title = taskPut.Title;
            existingTask.Description = taskPut.Description;
            existingTask.IsCompleted = taskPut.IsCompleted;
            existingTask.DueDate = taskPut.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TaskItemRepository.UpdateTaskAsync(existingTask);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

    }
}
