using System.ComponentModel.DataAnnotations;

namespace ShopForHome.ViewModels
{
    // Product ViewModels
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public decimal GST { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal GST { get; set; } = 18.00m;

        [Required]
        public int CategoryId { get; set; }

        public IFormFile ImageFile { get; set; }
    }

    public class UpdateProductViewModel
    {
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal GST { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string CurrentImagePath { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    // Add these ViewModels to your ViewModels folder:

    public class BulkUploadResultViewModel
    {
        public int TotalRows { get; set; }
        public int SuccessfulRows { get; set; }
        public int ErrorRows { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public bool HasErrors => ErrorRows > 0;
        public bool HasWarnings => Warnings.Any();
        public string Summary => $"Successfully processed {SuccessfulRows} out of {TotalRows} products";
    }

    public class BulkProductCsvModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public decimal GST { get; set; }
        public string ImageUrl { get; set; }
    }
}
