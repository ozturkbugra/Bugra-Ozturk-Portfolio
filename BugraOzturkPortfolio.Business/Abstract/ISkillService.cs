using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface ISkillService
    {
        Task<List<Skill>> GetAllSkillsAsync();
        Task<Skill?> GetSkillByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveSkillAsync(Skill model);
        Task<(bool Success, string Message)> DeleteSkillAsync(Guid id);
    }
}