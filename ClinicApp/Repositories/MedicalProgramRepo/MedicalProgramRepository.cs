using ClinicApp.Core;
using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.MedicalProgramRepo
{
    public class MedicalProgramRepository : BaseRepository<MedicalProgram>, IMedicalProgramRepository
    {

        public MedicalProgramRepository(ClinicMvcdbfirstContext context) : base(context) { }

        public async Task<MedicalProgram?> GetByIdWithDetailsAsync(int programId) =>
            await _context.MedicalPrograms
                .Include(p => p.Doctor)
                .Include(p => p.Patients)
                .FirstOrDefaultAsync(p => programId == p.Id);

        public async Task<List<MedicalProgram>> GetProgramsByPatientIdAsync(int patientId) =>
            await _context.MedicalPrograms
                .Include(mp => mp.Doctor).ThenInclude(d => d.User)
                .Where(mp => mp.Patients.Any(p => p.Id == patientId))
                .ToListAsync();

        public async Task<List<MedicalProgram>> GetProgramsByDoctorIdAsync(int doctorId) =>
            await _context.MedicalPrograms
                .Include(mp => mp.Doctor).ThenInclude(d => d.User)
                .Where(mp => mp.DoctorId == doctorId)
                .ToListAsync();

        public async Task<PaginatedResult<MedicalProgram>> GetPaginatedFilteredProgramsAsync(int pageNumber, int pageSize, 
            List<Expression<Func<MedicalProgram, bool>>> predicates)
        {
            int totalRecords;
            IQueryable<MedicalProgram> query = _context.MedicalPrograms
                .Include(p => p.Doctor).Include(p => p.Patients);

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

            return new PaginatedResult<MedicalProgram>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
