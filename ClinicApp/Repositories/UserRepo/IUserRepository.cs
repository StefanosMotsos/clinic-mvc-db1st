using ClinicApp.Core;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.UserRepo
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);

        Task<PaginatedResult<User>> GetUserAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates);
    }
}
