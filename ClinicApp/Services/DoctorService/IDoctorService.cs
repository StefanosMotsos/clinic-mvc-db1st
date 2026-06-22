using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;

namespace ClinicApp.Services.DoctorService
{
    public interface IDoctorService
    {
        Task<DoctorReadOnlyDTO> SaveDoctorAsync(DoctorSignupDTO dto);

        Task<DoctorReadOnlyDTO> UpdateDoctorAsync(DoctorUpdateDTO dto, int callerUserId);

        Task<bool> DeleteDoctorAsync(Guid uuid);
        Task<DoctorReadOnlyDTO> GetDoctorByUuidAsync(Guid uuid);

        Task<PaginatedResult<DoctorReadOnlyDTO>> GetPaginatedFilteredDoctorsAsync(int pageNumber, int pageSize, DoctorFiltersDTO filters);
    }
}
