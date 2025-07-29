using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Enumerations;

namespace TaskManagementApi.Core.Entities
{
    public class SubTaskItem
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

        // Foreign Key to Parent Task
        [Required]
        public int ParentTaskId { get; set; }
        public TaskItem ParentTask { get; set; } = default!; // Navigation property to parent task

        // Foreign Key to User (who created/owns this subtask)
        [Required]
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = default!;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    }
}
