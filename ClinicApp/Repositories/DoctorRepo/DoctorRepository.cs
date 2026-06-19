using ClinicApp.Core;
using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.DoctorRepo
{
    public class DoctorRepository : BaseAuditRepository<Doctor>, IDoctorRepository
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

        public async Task<Doctor?> GetDoctorByUserIdAsync(int userId)
        {
            Doctor? doctor = await _dbSet
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
            return doctor;
        }

        public async Task<PaginatedResult<Doctor>> GetPaginatedFilteredDoctorsAsync(int pageNumber, int pageSize, 
            List<Expression<Func<Doctor, bool>>> predicates)
        {
            int totalRecords;
            IQueryable<Doctor> query = _context.Doctors
                                            .Include(d => d.User)
                                            .ThenInclude(u => u.Role);

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
                .OrderBy(d => d.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Doctor>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public override async Task<Doctor?> GetByUuidAsync(Guid uuid) =>
            await _dbSet.Include(d => d.User).FirstOrDefaultAsync(d => d.Uuid == uuid);
    }
}
