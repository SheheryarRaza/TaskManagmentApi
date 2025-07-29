using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Enumerations;

namespace TaskManagementApi.Core.DTOs.DTO_Tasks
{
    public class DTO_TaskGet
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsNotificationEnabled { get; set; }
        public DateTime? NotificationDateTime { get; set; }
        public bool IsNotified { get; set; }
        public string? AssignedToUserName { get; set; }
        public string? AssignedByUserId { get; set; } // The ID of the user who assigned it
        public string? AssignedByUserName { get; set; }

        public TaskPriority Priority { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
