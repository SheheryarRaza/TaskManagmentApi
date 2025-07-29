using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Enumerations;

namespace TaskManagementApi.Core.DTOs.DTO_Subtask
{
    public class DTO_SubtaskPut
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskPriority Priority { get; set; }
    }
}
