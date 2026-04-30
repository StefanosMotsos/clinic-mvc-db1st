using ClinicApp.Core;
using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Repositories.UserRepo
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {

        public UserRepository(ClinicMvcdbfirstContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);

        public async Task<PaginatedResult<User>> GetUserAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates)
        {
            int totalRecords;
            IQueryable<User> query = _context.Users.Include(u => u.Role);

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }
            totalRecords = await query.CountAsync();
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<User>()
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }
    }
}
