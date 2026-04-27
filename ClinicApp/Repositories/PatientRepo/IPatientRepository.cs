using ClinicApp.Core;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.PatientRepo
{
    public interface IPatientRepository : IBaseRepository<Patient>
    {
        Task<List<MedicalProgram>> GetPatientProgramsAsync(int patientId);

        Task<Patient?> GetAMKAAsync(string? amka);

        Task<PaginatedResult<User>> GetPaginatedUsersPatientsAsync(int pageNumber, int pageSize);

        Task<PaginatedResult<Patient>> GetPaginatedFilteredUsersPatientAsync(int pageNumber, int pageSize,
            List<Expression<Func<Patient, bool>>> predicates);
    }
}
