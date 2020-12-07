using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models
{
    public class ProjectsModel
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
      
        public DateTime StartDate { get; set; }
       
        public DateTime DueDate { get; set; }
        public int TotalFiles { get; set; }
        public int TotalUsers { get; set; }
        public int Priority { get; set; }
    }
}
