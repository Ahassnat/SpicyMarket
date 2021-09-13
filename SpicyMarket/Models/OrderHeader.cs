using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpicyMarket.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public double OrderTotalOrginal { get; set; } // the orginal total without discount or copuon dicaount 
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")] // c => currency
        public double OrderTotal { get; set; }
        [Required]
        [Display(Name = "Pickup Time")]
        public DateTime PickUpTime { get; set; } //  for time
        [NotMapped]
        [Required]
        public DateTime PickUpDate { get; set; } // for Date
        [Display(Name = "Coupon Code")]
        public string CouponCode { get; set; }
        public double CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Pickup Name")]
        public string PickUpName { get; set; }
        [Display(Name = "Phone Number ")]
        public string PhoneNumber { get; set; }
        public string TransactionId { get; set; }
    }
}
