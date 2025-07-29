using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Core.Entities
{
    public class TaskItemTag
    {
        [Required]
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = default!;

        [Required]
        public int TagId { get; set; }
        public Tag Tag { get; set; } = default!;
    }
}
