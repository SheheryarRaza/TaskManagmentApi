using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagementApi.Core.DTOs.DTO_Tasks
{
    public class TaskQueryParams
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        [FromQuery(Name = "isCompleted")]
        public bool? IsCompleted { get; set; }

        [FromQuery(Name = "dueDateFrom")]
        public DateTime? DueDateFrom { get; set; }

        [FromQuery(Name = "dueDateTo")]
        public DateTime? DueDateTo { get; set; }

        [FromQuery(Name = "includeDeleted")] // NEW: Allow including soft-deleted tasks
        public bool IncludeDeleted { get; set; } = false;

        [FromQuery(Name = "includeNotified")] // NEW: Allow including tasks that have already been notified
        public bool IncludeNotified { get; set; } = false;

        //sorting
        [FromQuery(Name = "sortBy")]
        public string? SortBy { get; set; } = "dueDate"; // Default sorting by due date

        [FromQuery(Name = "sortOrder")]
        public string? SortOrder { get; set; } = "desc"; // Default sorting order is ascending

        //pagination
        [FromQuery(Name = "pageNumber")]
        public int PageNumber { get; set; } = 1; // Default page number

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 10; // Default page size

        private const int MaxPageSize = 40;
        public int AdjustedPageSize
        {
            get => Math.Min(PageSize, MaxPageSize);
            set => PageSize = value;
        }
    }
}
