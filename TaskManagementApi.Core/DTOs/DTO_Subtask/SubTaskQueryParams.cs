using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Core.Enumerations;

namespace TaskManagementApi.Core.DTOs.DTO_Subtask
{
    public class SubTaskQueryParams
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; } = null;

        [FromQuery(Name = "isCompleted")]
        public bool? IsCompleted { get; set; } = null;

        [FromQuery(Name = "dueDateFrom")]
        public DateTime? DueDateFrom { get; set; } = null;

        [FromQuery(Name = "dueDateTo")]
        public DateTime? DueDateTo { get; set; } = null;

        [FromQuery(Name = "includeDeleted")]
        public bool IncludeDeleted { get; set; } = false;

        [FromQuery(Name = "priority")]
        public TaskPriority? Priority { get; set; } = null;

        // Sorting
        [FromQuery(Name = "sortBy")]
        public string? SortBy { get; set; } = "CreatedAt";

        [FromQuery(Name = "sortOrder")]
        public string? SortOrder { get; set; } = "desc";

        // Pagination
        [FromQuery(Name = "pageNumber")]
        public int PageNumber { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 10;

        // Max page size to prevent abuse
        private const int MaxPageSize = 50;
        public int AdjustedPageSize
        {
            get => Math.Min(PageSize, MaxPageSize);
            set => PageSize = value;
        }
    }
}
