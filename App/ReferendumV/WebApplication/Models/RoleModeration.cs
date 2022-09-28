using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class RoleModeration
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string[] InsertIds { get; set; }
        public string[] DropIds { get; set; }
    }
}
