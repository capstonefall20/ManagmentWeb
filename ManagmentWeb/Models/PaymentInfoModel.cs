using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models
{
    public class PaymentInfoModel
    {
        public int PaymentId { get; set; }
        public string PayTo { get; set; }
        public int PaymentType { get; set; }
        public decimal Rate { get; set; }
        public decimal Hours { get; set; }
        public int PaymentMethod { get; set; }
        public decimal Total { get; set; }
    }
}
