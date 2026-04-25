using ClinicApp.Models;
using ClinicApp.Repositories.Base;

namespace ClinicApp.Repositories.MedicalProgramRepo
{
    public interface IMedicalProgramRepository : IBaseRepository<MedicalProgram>
    {
        Task<List<Patient>> GetProgramPatientsAsync(int programId);

        Task<Doctor?> GetProgramDoctorAsync(int programId);
    }
}
