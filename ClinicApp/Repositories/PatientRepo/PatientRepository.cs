using ClinicApp.Core;
using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.PatientRepo
{
    public class PatientRepository : BaseAuditRepository<Patient>, IPatientRepository
    {
        public PatientRepository(ClinicMvcdbfirstContext context) : base(context) { }

        public async Task<List<MedicalProgram>> GetPatientProgramsAsync(int patientId)
        {
            var programs = await _context.Patients
                .Where(p =>  p.Id == patientId)
                .SelectMany(p => p.Programs)
                .ToListAsync();

            return programs;
        }
        public async Task<Patient?> GetPatientByAMKAAsync(string? amka)
        {
            return await _context.Patients
                .Where(p => p.Amka == amka)
                .SingleOrDefaultAsync();
        }

        public async Task<Patient?> GetPatientByUserId(int userId)
        {
            return await _dbSet
                .Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<PaginatedResult<User>> GetPaginatedUsersPatientsAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            var usersWithRolePatient = await _context.Users
                .Include(u => u.Patient)
                .Include(u => u.Role)
                .Where(u => u.Patient != null)
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            int totalRecords = await _context.Users
                .Where(u => u.Patient != null)
                .CountAsync();

            return new PaginatedResult<User>(usersWithRolePatient, totalRecords, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Patient>> GetPaginatedFilteredPatientAsync(int pageNumber, int pageSize,
            List<Expression<Func<Patient, bool>>> predicates)
        {
            int totalRecords;
            IQueryable<Patient> query = _context.Patients
                                            .Include(p => p.User)
                                            .ThenInclude(u => u.Role);

            if (predicates != null && predicates.Count > 0)
            {
                foreach(var predicate in predicates) 
                { 
                    query = query.Where(predicate);
                }
            }

            totalRecords = await query.CountAsync();
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Patient>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public override async Task<Patient?> GetByUuidAsync(Guid uuid)
        {
            return await _dbSet.Include(p => p.User).FirstOrDefaultAsync(d => d.Uuid == uuid);
        }
    }
}
