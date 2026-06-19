using ClinicApp.Data;
using ClinicApp.Models;

namespace ClinicApp.Repositories.Base
{
    public class BaseAuditRepository<T> : BaseRepository<T> where T : class, BaseEntity
    {
        public BaseAuditRepository(ClinicMvcdbfirstContext context) : base(context)
        {
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            T? entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            entity.DeletedAt = DateTime.UtcNow;
            entity.IsDeleted = true;
            return true;
        }

        public virtual async Task<T?> GetByUuidAsync(Guid uuid) => await _dbSet.FindAsync(uuid);
    }
}
