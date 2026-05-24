using BugraOzturkPortfolio.Entities.Concrete;

public interface ISiteSettingService
{
    Task<SiteSetting?> GetSiteSettingAsync();
    Task<(bool Success, string Message)> UpdateSiteSettingAsync(SiteSetting model);
}