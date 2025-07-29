using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Enumerations;

namespace TaskManagementApi.Core.DTOs.DTO_Tasks
{
    public class DTO_TaskPut
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsNotificationEnabled { get; set; }
        public DateTime? NotificationDateTime { get; set; }

        public string? AssignedToUserId { get; set; }

        public TaskPriority Priority { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
