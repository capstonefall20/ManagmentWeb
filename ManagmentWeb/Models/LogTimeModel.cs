using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models
{
    public class LogTimeModel
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public List<SelectListItem> Users { get; set; }
        public string Who { get; set; }
        public string LogDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
        public int CreatedBy { get; set; }
    }
}
