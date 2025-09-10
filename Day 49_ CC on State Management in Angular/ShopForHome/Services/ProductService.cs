using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.Models;
using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    public class ProductService : IProductService
    {
        private readonly ShopForHomeDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(ShopForHomeDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<PagedResult<ProductViewModel>> GetProductsAsync(int pageNumber = 1, int pageSize = 10, int? categoryId = null)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    GST = p.GST,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImagePath = p.ImagePath,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<ProductViewModel>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<ProductViewModel> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) return null;

            return new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                GST = product.GST,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                ImagePath = product.ImagePath,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public async Task<(bool Success, string Message)> CreateProductAsync(CreateProductViewModel model)
        {
            string imagePath = null;
            if (model.ImageFile != null)
            {
                imagePath = await SaveProductImageAsync(model.ImageFile);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                GST = model.GST,
                CategoryId = model.CategoryId,
                ImagePath = imagePath
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return (true, "Product created successfully");
        }

        public async Task<(bool Success, string Message)> UpdateProductAsync(UpdateProductViewModel model)
        {
            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
                return (false, "Product not found");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.GST = model.GST;
            product.CategoryId = model.CategoryId;

            if (model.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImagePath.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                product.ImagePath = await SaveProductImageAsync(model.ImageFile);
            }

            await _context.SaveChangesAsync();
            return (true, "Product updated successfully");
        }

        public async Task<(bool Success, string Message)> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return (false, "Product not found");

            // Delete image if exists
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImagePath.TrimStart('/'));
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return (true, "Product deleted successfully");
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<string> SaveProductImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/assets/images/" + uniqueFileName;
        }

        public async Task<bool> ProductExistsByNameAsync(string productName)
        {
            return await _context.Products
                .AnyAsync(p => p.Name.ToLower() == productName.ToLower());
        }

        public async Task<(bool Success, string Message)> CreateProductFromCsvAsync(BulkProductCsvModel csvModel, int categoryId)
        {
            try
            {
                var product = new Product
                {
                    Name = csvModel.Name.Trim(),
                    Description = csvModel.Description?.Trim() ?? "",
                    Price = csvModel.Price,
                    StockQuantity = csvModel.StockQuantity,
                    GST = csvModel.GST > 0 ? csvModel.GST : 18.00m,
                    CategoryId = categoryId,
                    ImagePath = string.IsNullOrWhiteSpace(csvModel.ImageUrl) ? null : csvModel.ImageUrl.Trim()
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return (true, "Product created successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<BulkUploadResultViewModel> ProcessBulkUploadAsync(List<BulkProductCsvModel> csvRecords)
        {
            var result = new BulkUploadResultViewModel
            {
                TotalRows = csvRecords.Count
            };

            var errors = new List<string>();
            var warnings = new List<string>();

            try
            {
                // Get all categories for lookup
                var categories = await GetCategoriesAsync();
                var categoryLookup = categories.ToDictionary(c => c.Name.ToLower(), c => c);

                foreach (var (record, index) in csvRecords.Select((r, i) => (r, i)))
                {
                    var rowNumber = index + 2; // +2 for header and 0-based index

                    try
                    {
                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(record.Name))
                        {
                            errors.Add($"Row {rowNumber}: Product name is required");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(record.CategoryName))
                        {
                            errors.Add($"Row {rowNumber}: Category name is required");
                            continue;
                        }

                        if (record.Price <= 0)
                        {
                            errors.Add($"Row {rowNumber}: Price must be greater than 0");
                            continue;
                        }

                        if (record.StockQuantity < 0)
                        {
                            errors.Add($"Row {rowNumber}: Stock quantity cannot be negative");
                            continue;
                        }

                        // Find category
                        var categoryKey = record.CategoryName.Trim().ToLower();
                        if (!categoryLookup.TryGetValue(categoryKey, out var category))
                        {
                            errors.Add($"Row {rowNumber}: Category '{record.CategoryName}' not found. Available categories: {string.Join(", ", categories.Select(c => c.Name))}");
                            continue;
                        }

                        // Check if product exists
                        if (await ProductExistsByNameAsync(record.Name))
                        {
                            warnings.Add($"Row {rowNumber}: Product '{record.Name}' already exists, skipped");
                            continue;
                        }

                        // Create product
                        var createResult = await CreateProductFromCsvAsync(record, category.CategoryId);

                        if (createResult.Success)
                        {
                            result.SuccessfulRows++;
                        }
                        else
                        {
                            errors.Add($"Row {rowNumber}: {createResult.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowNumber}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Unexpected error: {ex.Message}");
            }

            result.ErrorRows = errors.Count;
            result.Errors = errors;
            result.Warnings = warnings;

            return result;
        }
    }
}
