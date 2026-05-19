using RT.Comb;

namespace BugraOzturkPortfolio.Entities.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Provider.Sql.Create();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}