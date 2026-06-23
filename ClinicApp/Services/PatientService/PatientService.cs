using AutoMapper;
using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;
using ClinicApp.Exceptions;
using ClinicApp.Models;
using ClinicApp.Repositories;
using ClinicApp.Resources;
using ClinicApp.Security;
using Serilog;
using System.Linq.Expressions;
using System.Numerics;

namespace ClinicApp.Services.PatientService
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;
        private readonly IEncryptionUtil _encryptionUtil;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PatientService> logger, IEncryptionUtil encryptionUtil)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _encryptionUtil = encryptionUtil;
        }

        public async Task<PatientReadOnlyDTO> SavePatientAsync(PatientSignupDTO dto)
        {

            try
            {
                Patient patient = _mapper.Map<Patient>(dto);
                User user = _mapper.Map<User>(dto);

                User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(dto.Username!);
                if (existingUser != null)
                {
                    _logger.LogWarning("Patient with username {Username} already exists", dto.Username);
                    throw new EntityAlreadyExistsException("Patient", ErrorMessages.AlreadyExists);
                }

                User? existingEmail = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email!);
                if (existingEmail != null)
                {
                    _logger.LogWarning("User with email {Email} already exists", dto.Email);
                    throw new EntityAlreadyExistsException("Email", ErrorMessages.AlreadyExists);
                }

                Patient? existingAmka = await _unitOfWork.PatientRepository.GetPatientByAMKAAsync(dto.Amka);
                if (existingAmka != null)
                {
                    _logger.LogWarning("User with amka {Amka} already exists", dto.Amka);
                    throw new EntityAlreadyExistsException("Amka", ErrorMessages.AlreadyExists);
                }

                Role? role = await _unitOfWork.RoleRepository.GetByIdAsync(4);
                user.RoleId = role!.Id;
                user.Role = role;

                user.Password = _encryptionUtil.Encrypt(user.Password);
                user.Patient = patient;

                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Patient with {Username} was registered successfully!", user.Username);
                return _mapper.Map<PatientReadOnlyDTO>(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving patient with username {Username}", dto.Username);
                throw;
            }
        }

        public async Task<PatientReadOnlyDTO> UpdatePatientAsync(PatientUpdateDTO dto, int callerUserId)
        {
            try
            {
                Patient? patient = await _unitOfWork.PatientRepository.GetPatientByUserId(callerUserId);
                if (patient is null)
                {
                    _logger.LogWarning("Patient with id {Id} not found", callerUserId);
                    throw new EntityNotFoundException("Patient", ErrorMessages.NotFound);
                }

                if (dto.Username != patient.User.Username)
                {
                    User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(dto.Username!);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("Patient with username {Username} already exists", dto.Username);
                        throw new EntityAlreadyExistsException("Patient", ErrorMessages.AlreadyExists);
                    }
                }

                if (dto.Email != patient.User.Email)
                {
                    User? existingEmail = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email!);
                    if (existingEmail != null)
                    {
                        _logger.LogWarning("Patient with email {Email} already exists", dto.Email);
                        throw new EntityAlreadyExistsException("Email", ErrorMessages.AlreadyExists);
                    }
                }

                if (dto.Amka != patient.Amka)
                {
                    Patient? existingAmka = await _unitOfWork.PatientRepository.GetPatientByAMKAAsync(dto.Amka);
                    if (existingAmka != null)
                    {
                        _logger.LogWarning("User with amka {Amka} already exists", dto.Amka);
                        throw new EntityAlreadyExistsException("Amka", ErrorMessages.AlreadyExists);
                    }
                }

                _mapper.Map(dto, patient);
                _mapper.Map(dto, patient.User);
                patient.User.Password = _encryptionUtil.Encrypt(patient.User.Password);
                patient.ModifiedAt = DateTime.UtcNow;
                patient.User.ModifiedAt = DateTime.UtcNow;

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Patient {Username} was updated successfully!", patient.User.Username);
                return _mapper.Map<PatientReadOnlyDTO>(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient with username {Username}", dto.Username);
                throw;
            }
        }

        public async Task<bool> DeletePatientAsync(Guid uuid)
        {
            Patient? patient = await _unitOfWork.PatientRepository.GetByUuidAsync(uuid);
            if (patient == null)
            {
                _logger.LogWarning("Patient with uuid {Uuid} was not found", uuid);
                throw new EntityNotFoundException("Patient", ErrorMessages.NotFound);
            }

            patient.IsDeleted = true;
            patient.DeletedAt = DateTime.UtcNow;
            patient.User.IsDeleted = true;
            patient.User.DeletedAt = DateTime.UtcNow;
            patient.ModifiedAt = DateTime.UtcNow;
            patient.User.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Patient with uuid {Uuid} was soft deleted successfully", uuid);
            return true;
        }

        public async Task<PatientReadOnlyDTO> GetPatientAsync(Guid uuid)
        {
            Patient? patient = await _unitOfWork.PatientRepository.GetByUuidAsync(uuid);
            if (patient == null)
            {
                _logger.LogWarning("Patient with uuid {Uuid} was not found", uuid);
                throw new EntityNotFoundException("Patient", ErrorMessages.NotFound);
            }

            _logger.LogInformation("Patient with uuid {Uuid} was fetched successfully!", uuid);
            return _mapper.Map<PatientReadOnlyDTO>(patient);
        }

        public async Task<PaginatedResult<PatientReadOnlyDTO>> GetPaginatedFilteredPatientAsync(int pageNumber, int pageSize, 
            PatientFiltersDTO filters)
        {
            List<Expression<Func<Patient, bool>>> predicates = [];
            if (!string.IsNullOrEmpty(filters.Username)) predicates.Add(p => p.User.Username == filters.Username);
            if (!string.IsNullOrEmpty(filters.Firstname)) predicates.Add(p => p.User.Firstname == filters.Firstname);
            if (!string.IsNullOrEmpty(filters.Lastname)) predicates.Add(p => p.User.Lastname.Contains(filters.Lastname));
            if (!string.IsNullOrEmpty(filters.Email)) predicates.Add(p => p.User.Email == filters.Email);
            if (!string.IsNullOrEmpty(filters.Amka)) predicates.Add(p => p.Amka == filters.Amka);
            if (!string.IsNullOrEmpty(filters.BloodType)) predicates.Add(p => p.BloodType == filters.BloodType);

            PaginatedResult<Patient> result = await _unitOfWork.PatientRepository.GetPaginatedFilteredPatientAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<PatientReadOnlyDTO>
            {
                Data = _mapper.Map<List<PatientReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
            };

            _logger.LogInformation("Retrieved {Count} Patients", dtoResult.Data.Count);
            return dtoResult;
        }
    }
}
