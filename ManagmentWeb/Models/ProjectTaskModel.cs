using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models
{
    public class ProjectTaskModel
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
        public int TotalUsers { get; set; }
        public int CreatedBy { get; set; }
        public List<SelectListItem> Users { get; set; }
        [Required]
        public int[] UsersId { get; set; }
    }
}
