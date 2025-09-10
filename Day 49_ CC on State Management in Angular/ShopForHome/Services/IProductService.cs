using ShopForHome.Models;
using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductViewModel>> GetProductsAsync(int pageNumber = 1, int pageSize = 10, int? categoryId = null);
        Task<ProductViewModel> GetProductByIdAsync(int productId);
        Task<(bool Success, string Message)> CreateProductAsync(CreateProductViewModel model);
        Task<(bool Success, string Message)> UpdateProductAsync(UpdateProductViewModel model);
        Task<(bool Success, string Message)> DeleteProductAsync(int productId);
        Task<List<Category>> GetCategoriesAsync();
        Task<string> SaveProductImageAsync(IFormFile imageFile);
    }
}
