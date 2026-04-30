using AutoMapper;
using ClinicApp.Core;
using ClinicApp.DTO;
using ClinicApp.Repositories.UoW;
using Serilog;

namespace ClinicApp.Services.PatientService
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PatientService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedPatientsAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.PatientRepository.GetPaginatedUsersPatientsAsync(pageNumber, pageSize);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = _mapper.Map<List<UserReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            _logger.LogInformation("Retrieved {Count} users-patients", dtoResult.Data.Count);
            return dtoResult;
        }
    }
}
