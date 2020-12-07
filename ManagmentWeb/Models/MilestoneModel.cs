using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models
{
    public class MilestoneModel
    {
        public int MileStoneId { get; set; }
        [Display(Name = "Task")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Due Date")]
        [Required]
        public string DueDate { get; set; }
        [Display(Name = "Start Date")]
        [Required]
        public string StartDate { get; set; }
        [Display(Name = "Expected Date")]
        [Required]
        public string ExpectedDate { get; set; }
        [Display(Name = "Responsible Person")]
        [Required]
        public int ResponsiblePersonId { get; set; }
        [Display(Name = "Responsible Person")]
        public string ResponsiblePerson { get; set; }
        public List<SelectListItem> Users { get; set; }
        public int CreatedBy { get; set; }
    }
}
