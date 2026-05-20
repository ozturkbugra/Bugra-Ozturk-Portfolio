namespace BugraOzturkPortfolio.DataAccess.Repositories.Abstract
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}