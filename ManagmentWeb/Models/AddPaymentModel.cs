using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Models
{
    public class AddPaymentModel
    {
        public int PaymentId { get; set; }
        [Required]
        public int PayTo { get; set; }
        [Required]
        [Display(Name = "Payment Type")]
        public int PaymentType { get; set; }
        
        public decimal Rate { get; set; }

        [Required]
        public decimal Hours { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public int PaymentMethod { get; set; }
        public int CreatedBy { get; set; }
        public decimal Total { get; set; }
       
    }
}
