using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;

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
    }
}
