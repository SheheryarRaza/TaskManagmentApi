using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Core.DTOs.DTO_Subtask
{
    public class DTO_SubtaskPost
    {
        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
