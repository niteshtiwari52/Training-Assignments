using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Models;
using ShopForHome.Services;
using ShopForHome.ViewModels;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;

namespace ShopForHome.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class ProductManagementController : Controller
    {
        private readonly IProductService _productService;
        public ProductManagementController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, int? categoryId = null)
        {
            try
            {
                var products = await _productService.GetProductsAsync(page, pageSize, categoryId);
                ViewBag.Categories = await _productService.GetCategoriesAsync();
                ViewBag.SelectedCategoryId = categoryId;
                return View(products);
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while loading products.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Categories = await _productService.GetCategoriesAsync();
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while loading categories.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    ViewBag.Categories = await _productService.GetCategoriesAsync();
                }
                catch
                {
                    ViewBag.Categories = new List<object>();
                }
                return View(model);
            }

            try
            {
                var result = await _productService.CreateProductAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    ViewBag.Categories = await _productService.GetCategoriesAsync();
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception here
                ModelState.AddModelError("", "An error occurred while creating the product.");
                try
                {
                    ViewBag.Categories = await _productService.GetCategoriesAsync();
                }
                catch
                {
                    ViewBag.Categories = new List<object>();
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                var model = new UpdateProductViewModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    GST = product.GST,
                    CategoryId = product.CategoryId,
                    CurrentImagePath = product.ImagePath
                };

                ViewBag.Categories = await _productService.GetCategoriesAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while loading product data.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    ViewBag.Categories = await _productService.GetCategoriesAsync();
                }
                catch
                {
                    ViewBag.Categories = new List<object>();
                }
                return View(model);
            }

            try
            {
                var result = await _productService.UpdateProductAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    ViewBag.Categories = await _productService.GetCategoriesAsync();
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception here
                ModelState.AddModelError("", "An error occurred while updating the product.");
                try
                {
                    ViewBag.Categories = await _productService.GetCategoriesAsync();
                }
                catch
                {
                    ViewBag.Categories = new List<object>();
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);

                if (result.Success)
                    TempData["SuccessMessage"] = result.Message;
                else
                    TempData["ErrorMessage"] = result.Message;
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                return View(product);
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while loading product details.";
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<IActionResult> BulkUpload()
        {
            try
            {
                ViewBag.Categories = await _productService.GetCategoriesAsync();
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading categories.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpload(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a CSV file to upload";
                try
                {
                    ViewBag.Categories = await _productService.GetCategoriesAsync();
                }
                catch
                {
                    ViewBag.Categories = new List<object>();
                }
                return View();
            }

            if (!csvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Only CSV files are allowed";
                try
                {
                    ViewBag.Categories = await _productService.GetCategoriesAsync();
                }
                catch
                {
                    ViewBag.Categories = new List<object>();
                }
                return View();
            }

            var result = await ProcessBulkUploadAsync(csvFile);
            return View("BulkUploadResult", result);
        }

        private async Task<BulkUploadResultViewModel> ProcessBulkUploadAsync(IFormFile csvFile)
        {
            var result = new BulkUploadResultViewModel();
            var errors = new List<string>();
            var warnings = new List<string>();

            try
            {
                using var reader = new StringReader(Encoding.UTF8.GetString(ReadAllBytes(csvFile)));
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    TrimOptions = TrimOptions.Trim
                });

                var records = new List<BulkProductCsvModel>();

                try
                {
                    records = csv.GetRecords<BulkProductCsvModel>().ToList();
                }
                catch (Exception ex)
                {
                    errors.Add($"Error reading CSV file: {ex.Message}");
                    result.Errors = errors;
                    return result;
                }

                result.TotalRows = records.Count;

                // Get all categories using service
                var categories = await _productService.GetCategoriesAsync();
                var categoryLookup = categories.ToDictionary(c => c.Name.ToLower(), c => c);

                foreach (var (record, index) in records.Select((r, i) => (r, i)))
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
                            errors.Add($"Row {rowNumber}: Category '{record.CategoryName}' not found. Available categories: {string.Join(", ", categoryLookup.Keys)}");
                            continue;
                        }

                        // Create product model for service
                        var createModel = new CreateProductViewModel
                        {
                            Name = record.Name.Trim(),
                            Description = record.Description?.Trim() ?? "",
                            Price = record.Price,
                            StockQuantity = record.StockQuantity,
                            GST = record.GST > 0 ? record.GST : 18.00m,
                            CategoryId = category.CategoryId
                            // Note: ImageFile is not set here as we're dealing with URL in CSV
                        };

                        // Check if product already exists by trying to get it
                        // This is a simple check - you might want to add a method to your service for this
                        var existingProducts = await _productService.GetProductsAsync(1, 1000); // Get all products to check
                        var exists = existingProducts.Items.Any(p => p.Name.Equals(record.Name.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (exists)
                        {
                            warnings.Add($"Row {rowNumber}: Product '{record.Name}' already exists, skipped");
                            continue;
                        }

                        // Use service to create product
                        var createResult = await _productService.CreateProductAsync(createModel);

                        if (createResult.Success)
                        {
                            result.SuccessfulRows++;

                            // If there's an ImageUrl, you might want to update the product
                            // This would require additional service methods to handle image URLs
                            if (!string.IsNullOrWhiteSpace(record.ImageUrl))
                            {
                                warnings.Add($"Row {rowNumber}: Product '{record.Name}' created but image URL ignored. Please upload images manually.");
                            }
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

                result.ErrorRows = errors.Count;
                result.Errors = errors;
                result.Warnings = warnings;
            }
            catch (Exception ex)
            {
                errors.Add($"Unexpected error: {ex.Message}");
                result.Errors = errors;
            }

            return result;
        }

        private static byte[] ReadAllBytes(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    //[HttpGet]
    //public IActionResult BulkUpload()
    //{
    //    return View();
    //}

    //[HttpPost]
    //public async Task<IActionResult> BulkUpload(IFormFile csvFile)
    //{
    //    if (csvFile == null || csvFile.Length == 0)
    //    {
    //        TempData["ErrorMessage"] = "Please select a CSV file to upload";
    //        return View();
    //    }

    //    if (!csvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
    //    {
    //        TempData["ErrorMessage"] = "Only CSV files are allowed";
    //        return View();
    //    }

    //    var result = await ProcessBulkUploadAsync(csvFile);
    //    return View("BulkUploadResult", result);
    //}

    //private async Task<BulkUploadResultViewModel> ProcessBulkUploadAsync(IFormFile csvFile)
    //{
    //    var result = new BulkUploadResultViewModel();
    //    var errors = new List<string>();
    //    var warnings = new List<string>();

    //    try
    //    {
    //        using var reader = new StringReader(Encoding.UTF8.GetString(ReadAllBytes(csvFile)));
    //        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
    //        {
    //            HeaderValidated = null,
    //            MissingFieldFound = null,
    //            TrimOptions = TrimOptions.Trim
    //        });

    //        var records = new List<BulkProductCsvModel>();

    //        try
    //        {
    //            records = csv.GetRecords<BulkProductCsvModel>().ToList();
    //        }
    //        catch (Exception ex)
    //        {
    //            errors.Add($"Error reading CSV file: {ex.Message}");
    //            result.Errors = errors;
    //            return result;
    //        }

    //        result.TotalRows = records.Count;

    //        // Get all categories for lookup
    //        var categories = await _context.Categories.ToDictionaryAsync(c => c.Name.ToLower(), c => c);

    //        foreach (var (record, index) in records.Select((r, i) => (r, i)))
    //        {
    //            var rowNumber = index + 2; // +2 for header and 0-based index

    //            try
    //            {
    //                // Validate required fields
    //                if (string.IsNullOrWhiteSpace(record.Name))
    //                {
    //                    errors.Add($"Row {rowNumber}: Product name is required");
    //                    continue;
    //                }

    //                if (string.IsNullOrWhiteSpace(record.CategoryName))
    //                {
    //                    errors.Add($"Row {rowNumber}: Category name is required");
    //                    continue;
    //                }

    //                if (record.Price <= 0)
    //                {
    //                    errors.Add($"Row {rowNumber}: Price must be greater than 0");
    //                    continue;
    //                }

    //                if (record.StockQuantity < 0)
    //                {
    //                    errors.Add($"Row {rowNumber}: Stock quantity cannot be negative");
    //                    continue;
    //                }

    //                // Find category
    //                var categoryKey = record.CategoryName.Trim().ToLower();
    //                if (!categories.TryGetValue(categoryKey, out var category))
    //                {
    //                    errors.Add($"Row {rowNumber}: Category '{record.CategoryName}' not found. Available categories: {string.Join(", ", categories.Keys)}");
    //                    continue;
    //                }

    //                // Check if product exists
    //                var existingProduct = await _context.Products
    //                    .FirstOrDefaultAsync(p => p.Name.ToLower() == record.Name.Trim().ToLower());

    //                if (existingProduct != null)
    //                {
    //                    warnings.Add($"Row {rowNumber}: Product '{record.Name}' already exists, skipped");
    //                    continue;
    //                }

    //                var product = new Product
    //                {
    //                    Name = record.Name.Trim(),
    //                    Description = record.Description?.Trim() ?? "",
    //                    Price = record.Price,
    //                    StockQuantity = record.StockQuantity,
    //                    GST = record.GST > 0 ? record.GST : 18.00m,
    //                    CategoryId = category.CategoryId,
    //                    ImagePath = string.IsNullOrWhiteSpace(record.ImageUrl) ? null : record.ImageUrl.Trim()
    //                };

    //                _context.Products.Add(product);
    //                result.SuccessfulRows++;
    //            }
    //            catch (Exception ex)
    //            {
    //                errors.Add($"Row {rowNumber}: {ex.Message}");
    //            }
    //        }

    //        if (result.SuccessfulRows > 0)
    //        {
    //            await _context.SaveChangesAsync();
    //        }

    //        result.ErrorRows = errors.Count;
    //        result.Errors = errors;
    //        result.Warnings = warnings;
    //    }
    //    catch (Exception ex)
    //    {
    //        errors.Add($"Unexpected error: {ex.Message}");
    //        result.Errors = errors;
    //    }

    //    return result;
    //}

    //private static byte[] ReadAllBytes(IFormFile file)
    //{
    //    using var memoryStream = new MemoryStream();
    //    file.CopyTo(memoryStream);
    //    return memoryStream.ToArray();
    //}


}
