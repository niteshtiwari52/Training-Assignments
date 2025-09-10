using System.ComponentModel.DataAnnotations;

namespace ShopForHome.ViewModels
{
    // User Management ViewModels
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public List<int> RoleIds { get; set; } = new List<int>();
    }

    public class UpdateUserViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
