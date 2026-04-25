using ClinicApp.Core;
using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.DoctorRepo
{
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicMvcdbfirstContext context) : base(context) { }

        public async Task<List<MedicalProgram>> GetDoctorProgramsAsync(int doctorId)
        {
            List<MedicalProgram> programs;

            programs = await _context.MedicalPrograms
                .Where(p => p.DoctorId == doctorId)
                .ToListAsync();

            return programs;
        }

        public async Task<User?> GetUserDoctorByUsernameAsync(string username)
        {
            var userDoctor =  await _context.Users
                .Include(u => u.Doctor)
                .Where(u => u.Username == username && u.Doctor != null)
                .SingleOrDefaultAsync();

            return userDoctor;
        }

        public async Task<PaginatedResult<User>> GetDoctorsAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates)
        {

            int totalRecords;
            IQueryable<User> query = _context.Users
                .Include(u => u.Doctor)
                .Where(u => u.Doctor != null);

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
