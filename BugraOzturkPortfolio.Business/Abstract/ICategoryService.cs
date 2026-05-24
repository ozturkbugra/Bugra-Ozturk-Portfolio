using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveCategoryAsync(Category model);
        Task<(bool Success, string Message)> DeleteCategoryAsync(Guid id);
    }
}