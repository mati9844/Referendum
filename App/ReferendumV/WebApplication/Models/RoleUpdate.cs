using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Areas.Identity.Data;

namespace WebApplication.Models
{
    public class RoleUpdate
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<WebApplicationUser> Members { get; set; }
        public IEnumerable<WebApplicationUser> NonMembers { get; set; }
    }
}
