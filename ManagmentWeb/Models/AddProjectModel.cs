using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models
{
    public class AddProjectModel
    {
        public int ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
       
        [Required]
        public string StartDate { get; set; }
        
        [Required]
        public string DueDate { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public int Status { get; set; }
        public List<SelectListItem> Users { get; set; }
        [Required]
        public int[] UsersId { get; set; }
        public int CreatedBy { get; set; }
        public List<IFormFile> files { get; set; }
        public List<ProjectFileModel> Filelist { get; set; }
    }
}
