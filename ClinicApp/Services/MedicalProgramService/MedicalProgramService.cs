using AutoMapper;
using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;
using ClinicApp.Exceptions;
using ClinicApp.Models;
using ClinicApp.Repositories;
using ClinicApp.Resources;
using System.Linq.Expressions;

namespace ClinicApp.Services.MedicalProgramService
{
    public class MedicalProgramService : IMedicalProgramService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MedicalProgramService> _logger;

        public MedicalProgramService(IMapper mapper, IUnitOfWork unitOfWork, ILogger<MedicalProgramService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MedicalProgramReadOnlyDTO> CreateProgramAsync(MedicalProgramCreateDTO dto)
        {
            try
            {
                MedicalProgram program = _mapper.Map<MedicalProgram>(dto);

                Doctor? doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(dto.DoctorId!.Value);
                if (doctor is null)
                {
                    _logger.LogWarning("Doctor with id {Id} not found", dto.DoctorId);
                    throw new EntityNotFoundException("Doctor", ErrorMessages.NotFound);
                }

                await _unitOfWork.MedicalProgramRepository.AddAsync(program);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Medical program {Title} was created successfully!", program.Title);
                return _mapper.Map<MedicalProgramReadOnlyDTO>(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medical program {Title}", dto.Title);
                throw;
            }
        }

        public async Task<MedicalProgramReadOnlyDTO> UpdateProgramAsync(MedicalProgramUpdateDTO dto, int id)
        {
            try
            {
                MedicalProgram? program = await _unitOfWork.MedicalProgramRepository.GetByIdAsync(id);
                if (program is null)
                {
                    _logger.LogWarning("Medical program with id {Id} not found", id);
                    throw new EntityNotFoundException("MedicalProgram", ErrorMessages.NotFound);
                }

                _mapper.Map(dto, program);

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Medical program {Title} was updated successfully!", program.Title);
                return _mapper.Map<MedicalProgramReadOnlyDTO>(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical program with id {Id}", id);
                throw;
            }
        }

        public async Task<MedicalProgramReadOnlyDTO> GetProgramByIdAsync(int id)
        {
            MedicalProgram? program = await _unitOfWork.MedicalProgramRepository.GetByIdWithDetailsAsync(id);
            if (program is null)
            {
                _logger.LogWarning("Medical program with id {Id} not found", id);
                throw new EntityNotFoundException("MedicalProgram", ErrorMessages.NotFound);
            }

            _logger.LogInformation("Medical program with id {Id} was fetched successfully!", id);
            return _mapper.Map<MedicalProgramReadOnlyDTO>(program);
        }

        public async Task<PaginatedResult<MedicalProgramReadOnlyDTO>> GetPaginatedFilteredProgramsAsync(int pageNumber, int pageSize,
            ProgramFiltersDTO filters)
        {
            List<Expression<Func<MedicalProgram, bool>>> predicates = [];
            if (!string.IsNullOrEmpty(filters.Title)) predicates.Add(mp => mp.Title.Contains(filters.Title));

            PaginatedResult<MedicalProgram> result = await _unitOfWork.MedicalProgramRepository.GetPaginatedFilteredProgramsAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<MedicalProgramReadOnlyDTO>
            {
                Data = _mapper.Map<List<MedicalProgramReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
            };

            _logger.LogInformation("Retrieved {Count} Medical Programs", dtoResult.Data.Count);
            return dtoResult;
        }

        public async Task<List<MedicalProgramReadOnlyDTO>> GetProgramsByDoctorIdAsync(int doctorId) 
        {
            List<MedicalProgram> programs = await _unitOfWork.MedicalProgramRepository.GetProgramsByDoctorIdAsync(doctorId);
            return _mapper.Map<List<MedicalProgramReadOnlyDTO>>(programs);
        }

        public async Task<List<MedicalProgramReadOnlyDTO>> GetProgramsByPatientIdAsync(int patientId) 
        { 
            List<MedicalProgram> programs = await _unitOfWork.MedicalProgramRepository.GetProgramsByPatientIdAsync(patientId);
            return _mapper.Map<List<MedicalProgramReadOnlyDTO>>(programs);
        }
    }
}
