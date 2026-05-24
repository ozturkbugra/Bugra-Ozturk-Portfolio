using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

public class SiteSettingService : ISiteSettingService
{
    private readonly IUnitOfWork _unitOfWork;
    public SiteSettingService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<SiteSetting?> GetSiteSettingAsync()
    {
        var repo = _unitOfWork.GetRepository<SiteSetting>();
        return (await repo.GetAllAsync()).FirstOrDefault();
    }

    public async Task<(bool Success, string Message)> UpdateSiteSettingAsync(SiteSetting model)
    {
        var repo = _unitOfWork.GetRepository<SiteSetting>();
        var exist = (await repo.GetAllAsync()).FirstOrDefault();

        if (exist == null)
        {
            await repo.AddAsync(model);
        }
        else
        {
            exist.MetaTitle = model.MetaTitle;
            exist.MetaKeywords = model.MetaKeywords;
            exist.MetaDescription = model.MetaDescription;
            exist.FaviconUrl = model.FaviconUrl;
            exist.AppleTouchIconUrl = model.AppleTouchIconUrl;
            exist.UpdatedDate = DateTime.UtcNow;
            repo.Update(exist);
        }
        await _unitOfWork.SaveChangesAsync();
        return (true, "Site ayarları başarıyla güncellendi.");
    }
}