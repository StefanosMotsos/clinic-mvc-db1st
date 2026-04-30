using ClinicApp.Core;
using ClinicApp.DTO;

namespace ClinicApp.Services.PatientService
{
    public interface IPatientService
    {

        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedPatientsAsync(int pageNumber, int pageSize);

    }
}
