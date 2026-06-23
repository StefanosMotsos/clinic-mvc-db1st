using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;

namespace ClinicApp.Services.MedicalProgramService
{
    public interface IMedicalProgramService
    {
        Task<MedicalProgramReadOnlyDTO> CreateProgramAsync(MedicalProgramCreateDTO dto);
        Task<MedicalProgramReadOnlyDTO> UpdateProgramAsync(MedicalProgramUpdateDTO dto, int id);
        Task<MedicalProgramReadOnlyDTO> GetProgramByIdAsync(int id);
        Task<PaginatedResult<MedicalProgramReadOnlyDTO>> GetPaginatedFilteredProgramsAsync(int pageNumber, int pageSize,
            ProgramFiltersDTO filters);
        Task<List<MedicalProgramReadOnlyDTO>> GetProgramsByDoctorIdAsync(int doctorId);
        Task<List<MedicalProgramReadOnlyDTO>> GetProgramsByPatientIdAsync(int patientId);

    }
}
