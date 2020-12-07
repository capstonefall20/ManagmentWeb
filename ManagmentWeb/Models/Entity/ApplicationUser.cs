using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models.Entity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
     
        public bool? Active { get; set; }
        public string Mobile { get; set; }
        public string Avatar { get; set; }
      
        public bool? Deleted { get; set; }

        public decimal HoureRate { get; set; }
    }
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base()
        {

        }
    }
}
