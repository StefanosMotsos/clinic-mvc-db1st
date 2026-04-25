namespace ClinicApp.Repositories.Base
{
    public interface IBaseRepository<T>
    {
        Task AddAsync(T Entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T Entity);

        Task<bool> DeleteAsync(int id);

        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<int> GetCountAsync();
    }
}
