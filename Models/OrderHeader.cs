using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]          //user id is working as FK 
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public double OrderTotalOriginal { get; set; }       //before coupon code

        [Required]
        [DisplayFormat(DataFormatString ="{0:C}")]
        [Display(Name ="Total Order")]
        public double OrderTotal { get; set; }       //after coupon code

        [Required]
        [Display(Name ="PickUpTime")]
        public DateTime PickUpTime { get; set; }

        [Required]
        [NotMapped]
        public DateTime PickUpDate { get; set; }

        [Display(Name ="Coupon Code")]
        public string CouponCode { get; set; }          //to store user saved coupon code for later use
        public double CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }


        [Display(Name = "PickUp Name")]
        public string PickUpName { get; set; }
        
        [Display(Name = "PickUp Phone Number")]
        public string PhoneNumber { get; set; }

        public string TransactionID { get; set; }       //important
    }
}
