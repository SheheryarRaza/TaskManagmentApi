using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TaskManagementApi.Core.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(256)]
        public override string UserName
        {
            get => base.UserName!;
            set => base.UserName = value;
        }

        [Required]
        [StringLength(256)]
        public override string Email
        {
            get => base.Email!;
            set => base.Email = value;
        }
    }
}
