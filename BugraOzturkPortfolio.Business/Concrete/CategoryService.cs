using System.Text.RegularExpressions;
using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var repo = _unitOfWork.GetRepository<Category>();
            var categories = await repo.GetAllAsync();
            return categories.Where(x => !x.IsDeleted).ToList();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Category>();
            return await repo.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> SaveCategoryAsync(Category model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return (false, "Lütfen kategori adını doldurunuz!");

            var repo = _unitOfWork.GetRepository<Category>();

            var allCategories = await repo.GetAllAsync();
            var isDuplicate = allCategories.Any(x =>
                x.Name.ToLower() == model.Name.ToLower() &&
                x.Id != model.Id &&
                !x.IsDeleted);

            if (isDuplicate)
                return (false, "Bu isimde zaten bir kategori mevcut!");

            model.Slug = GenerateSlug(model.Name);

            if (model.Id == Guid.Empty || model.Id == default)
            {
                model.IsDeleted = false;
                await repo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Kategori başarıyla eklendi.");
            }
            else
            {
                var existCategory = await repo.GetByIdAsync(model.Id);
                if (existCategory == null) return (false, "Güncellenecek kategori bulunamadı!");

                existCategory.Name = model.Name;
                existCategory.Slug = model.Slug;
                existCategory.UpdatedDate = DateTime.UtcNow;

                repo.Update(existCategory);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Kategori başarıyla güncellendi.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Category>();
            var existCategory = await repo.GetByIdAsync(id);

            if (existCategory == null)
                return (false, "Silinecek kategori bulunamadı!");

            var mappingRepo = _unitOfWork.GetRepository<ProjectCategoryMapping>();
            var mappings = await mappingRepo.GetAllAsync();

            if (mappings.Any(x => x.CategoryId == id))
                return (false, "Bu kategori projelere bağlı olduğu için silinemez!");

            existCategory.IsDeleted = true;
            existCategory.UpdatedDate = DateTime.UtcNow;

            repo.Update(existCategory);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Kategori başarıyla arşivlendi.");
        }

        // Türkçe karakterleri temizleyen ve SEO uyumlu URL üreten metot
        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLowerInvariant();

            // Türkçe karakter dönüşümleri
            str = str.Replace("ı", "i")
                     .Replace("ğ", "g")
                     .Replace("ü", "u")
                     .Replace("ş", "s")
                     .Replace("ö", "o")
                     .Replace("ç", "c");

            // Geçersiz karakterleri temizle
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // Birden fazla boşluğu teke indir
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // Boşlukları tireye çevir
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }
    }
}