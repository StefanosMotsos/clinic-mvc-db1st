using ClinicApp.Core;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.MedicalProgramRepo
{
    public interface IMedicalProgramRepository : IBaseRepository<MedicalProgram>
    {
        Task<MedicalProgram?> GetByIdWithDetailsAsync(int programId);

        Task<PaginatedResult<MedicalProgram>> GetPaginatedFilteredProgramsAsync(int pageNumber, int pageSize,
            List<Expression<Func<MedicalProgram, bool>>> predicates);

        Task<List<MedicalProgram>> GetProgramsByPatientIdAsync(int patientId);

        Task<List<MedicalProgram>> GetProgramsByDoctorIdAsync(int doctorId);
    }
}
