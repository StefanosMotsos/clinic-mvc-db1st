using ClinicApp.Core;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using System.Linq.Expressions;

namespace ClinicApp.Repositories.DoctorRepo
{
    public interface IDoctorRepository : IBaseRepository<Doctor>
    {
        Task<List<MedicalProgram>> GetDoctorProgramsAsync(int doctorId);

        Task<User?> GetUserDoctorByUsernameAsync(string username);

        Task<PaginatedResult<User>> GetDoctorsAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates);
    }
}
