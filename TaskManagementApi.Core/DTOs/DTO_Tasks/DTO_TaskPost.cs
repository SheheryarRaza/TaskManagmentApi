using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Enumerations;

namespace TaskManagementApi.Core.DTOs.DTO_Tasks
{
    public class DTO_TaskPost
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? DueDate { get; set; }

        public bool IsNotificationEnabled { get; set; } = false;
        public DateTime? NotificationDateTime { get; set; }
        //public string UserId { get; set; } = string.Empty;
        public string? AssignedToUserId { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public List<string> Tags { get; set; } = new List<string>();

    }
}
