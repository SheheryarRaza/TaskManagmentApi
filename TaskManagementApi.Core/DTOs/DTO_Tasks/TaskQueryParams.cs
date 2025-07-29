using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Core.Enumerations;

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

        [FromQuery(Name = "includeDeleted")]
        public bool IncludeDeleted { get; set; } = false;

        [FromQuery(Name = "includeNotified")]
        public bool IncludeNotified { get; set; } = false;

        [FromQuery(Name = "priority")]
        public TaskPriority? Priority { get; set; } = null;

        [FromQuery(Name = "tags")]
        public List<string> Tags { get; set; } = new List<string>();

        //sorting
        [FromQuery(Name = "sortBy")]
        public string? SortBy { get; set; } = "dueDate";

        [FromQuery(Name = "sortOrder")]
        public string? SortOrder { get; set; } = "desc";

        //pagination
        [FromQuery(Name = "pageNumber")]
        public int PageNumber { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 10;

        private const int MaxPageSize = 40;
        public int AdjustedPageSize
        {
            get => Math.Min(PageSize, MaxPageSize);
            set => PageSize = value;
        }
    }
}
