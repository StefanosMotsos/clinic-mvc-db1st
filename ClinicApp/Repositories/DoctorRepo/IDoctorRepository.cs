using ClinicApp.Core;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.DoctorRepo
{
    public interface IDoctorRepository : IBaseAuditRepository<Doctor>
    {
        Task<List<MedicalProgram>> GetDoctorProgramsAsync(int doctorId);

        Task<User?> GetUserDoctorByUsernameAsync(string username);

        Task<Doctor?> GetDoctorByUserIdAsync(int userId);

        Task<PaginatedResult<Doctor>> GetPaginatedFilteredDoctorsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Doctor, bool>>> predicates);
    }
}
