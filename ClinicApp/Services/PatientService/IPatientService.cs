using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;

namespace ClinicApp.Services.PatientService
{
    public interface IPatientService
    {
        Task<PatientReadOnlyDTO> SavePatientAsync(PatientSignupDTO dto);
        Task<PatientReadOnlyDTO> UpdatePatientAsync(PatientUpdateDTO dto, int callerUserId);
        Task<bool> DeletePatientAsync(Guid uuid);
        Task<PatientReadOnlyDTO> GetPatientAsync(Guid uuid);
        Task<PaginatedResult<PatientReadOnlyDTO>> GetPaginatedFilteredPatientAsync(int pageNumber, int pageSize,
            PatientFiltersDTO filters);
    }
}
