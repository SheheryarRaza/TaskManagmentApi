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
        [StringLength(256)] // Max length for UserName in IdentityUser
        public override string UserName
        {
            get => base.UserName!;
            set => base.UserName = value;
        }

        [Required]
        [StringLength(256)] // Max length for Email in IdentityUser
        public override string Email
        {
            get => base.Email!;
            set => base.Email = value;
        }
    }
}
