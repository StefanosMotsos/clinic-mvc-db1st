using ClinicApp.Models;
using ClinicApp.Repositories.Base;

namespace ClinicApp.Repositories.MedicalProgramRepo
{
    public interface IMedicalProgramRepository : IBaseRepository<MedicalProgram>
    {
        Task<MedicalProgram?> GetByIdWithDetailsAsync(int programId);
    }
}
