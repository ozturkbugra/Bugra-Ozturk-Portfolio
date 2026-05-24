using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface ISiteScriptService
    {
        Task<List<SiteScript>> GetAllScriptsAsync();
        Task<List<SiteScript>> GetActiveScriptsByPositionAsync(ScriptPosition position);
        Task<SiteScript?> GetScriptByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveScriptAsync(SiteScript model);
        Task<(bool Success, string Message)> DeleteScriptAsync(Guid id);
        Task<(bool Success, string Message)> ToggleStatusAsync(Guid id);
    }
}