using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Enumerations;

namespace TaskManagementApi.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Title { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;

        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public bool IsNotificationEnabled { get; set; } = false;
        public DateTime? NotificationDateTime { get; set; }
        public bool IsNotified { get; set; } = false;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        public string? AssignedByUserId { get; set; }
        public User? AssignedByUser { get; set; }
        public ICollection<SubTaskItem> SubTasks { get; set; } = new List<SubTaskItem>();

        public TaskPriority Priority { get; set; } = TaskPriority.Medium; // Default to Medium

        // NEW: Tags for the task (many-to-many relationship via TaskItemTag)
        public ICollection<TaskItemTag> TaskItemTags { get; set; } = new List<TaskItemTag>();
    }
}
