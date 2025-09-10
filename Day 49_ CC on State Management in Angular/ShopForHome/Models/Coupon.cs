using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopForHome.Models
{
    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        [Required]
        public bool IsPublic { get; set; } = false;

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        public int MaxUses { get; set; } = 1;

        public int TotalUsed { get; set; } = 0;

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey(nameof(CreatedBy))]
        public virtual User Creator { get; set; }

        public virtual ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
