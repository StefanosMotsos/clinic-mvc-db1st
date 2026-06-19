using ClinicApp.Core;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.PatientRepo
{
    public interface IPatientRepository : IBaseAuditRepository<Patient>
    {
        Task<List<MedicalProgram>> GetPatientProgramsAsync(int patientId);

        Task<Patient?> GetPatientByAMKAAsync(string? amka);

        Task<Patient?> GetPatientByUserId(int userId);

        Task<PaginatedResult<User>> GetPaginatedUsersPatientsAsync(int pageNumber, int pageSize);

        Task<PaginatedResult<Patient>> GetPaginatedFilteredPatientAsync(int pageNumber, int pageSize,
            List<Expression<Func<Patient, bool>>> predicates);
    }
}
