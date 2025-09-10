using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopForHome.Models;
using ShopForHome.Services;
using ShopForHome.ViewModels;

namespace ShopForHome.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null)
        {
            try
            {
                var products = await _productService.GetProductsAsync(page, pageSize, categoryId);

                var productDtos = products.Items.Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    GST = p.GST,
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName,
                    ImagePath = p.ImagePath
                }).ToList();

                var result = new PagedResult<ProductDto>
                {
                    Items = productDtos,
                    TotalCount = products.TotalCount,
                    PageNumber = products.PageNumber,
                    PageSize = products.PageSize,
                    TotalPages = products.TotalPages
                };

                return Ok(new ApiResponse<PagedResult<ProductDto>>
                {
                    Success = true,
                    Message = "Products retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PagedResult<ProductDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving products",
                    Data = null
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound(new ApiResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Product not found",
                        Data = null
                    });
                }

                var productDto = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    GST = product.GST,
                    CategoryId = product.CategoryId,
                    CategoryName = product.CategoryName,
                    ImagePath = product.ImagePath
                };

                return Ok(new ApiResponse<ProductDto>
                {
                    Success = true,
                    Message = "Product retrieved successfully",
                    Data = productDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the product",
                    Data = null
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponse<List<Category>>>> GetCategories()
        {
            try
            {
                var categories = await _productService.GetCategoriesAsync();

                return Ok(new ApiResponse<List<Category>>
                {
                    Success = true,
                    Message = "Categories retrieved successfully",
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<Category>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving categories",
                    Data = null
                });
            }
        }
    }
}
