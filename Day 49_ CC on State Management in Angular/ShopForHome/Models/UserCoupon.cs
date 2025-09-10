using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopForHome.Models
{
    [Table("UserCoupons")]
    public class UserCoupon
    {
        [Key]
        public int UserCouponId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CouponId { get; set; }

        [Required]
        public bool IsUsed { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(CouponId))]
        public virtual Coupon Coupon { get; set; }
    }
}
