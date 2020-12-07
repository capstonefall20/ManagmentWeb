using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models
{
    public class TaskUserModel
    {
        public int TaskUserId { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
    }
}
