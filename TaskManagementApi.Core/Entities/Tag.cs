using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Core.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation property for the many-to-many relationship with TaskItem
        public ICollection<TaskItemTag> TaskItemTags { get; set; } = new List<TaskItemTag>();
    }
}
