using AutoMapper;
using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;
using ClinicApp.Exceptions;
using ClinicApp.Models;
using ClinicApp.Repositories;
using ClinicApp.Resources;
using ClinicApp.Security;
using System.Linq.Expressions;
using System.Numerics;

namespace ClinicApp.Services.DoctorService
{
    public class DoctorService : IDoctorService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper,  IEncryptionUtil encryptionUtil, ILogger<DoctorService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _encryptionUtil = encryptionUtil;
            _logger = logger;
        }

        public async Task<DoctorReadOnlyDTO> SaveDoctorAsync(DoctorSignupDTO dto)
        {
            try
            {
                Doctor doctor = _mapper.Map<Doctor>(dto);
                User user = _mapper.Map<User>(dto);

                User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(dto.Username!);
                if (existingUser != null)
                {
                    _logger.LogWarning("Doctor with username {Username} already exists", dto.Username);
                    throw new EntityAlreadyExistsException("Doctor", ErrorMessages.AlreadyExists);
                }

                User? existingEmail = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email!);
                if (existingEmail != null)
                {
                    _logger.LogWarning("User with email {Email} already exists", dto.Email);
                    throw new EntityAlreadyExistsException("Email", ErrorMessages.AlreadyExists);
                }

                Role? role = await _unitOfWork.RoleRepository.GetByIdAsync(3);
                user.RoleId = role!.Id;
                user.Role = role;

                user.Password = _encryptionUtil.Encrypt(user.Password);
                user.Doctor = doctor;

                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Doctor {Username} was registered successfully!", user.Username);
                return _mapper.Map<DoctorReadOnlyDTO>(doctor);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving doctor with username {Username}", dto.Username);
                throw;
            }
        }

        public async Task<DoctorReadOnlyDTO> UpdateDoctorAsync(DoctorUpdateDTO dto, int callerUserId)
        {
            try
            {
                Doctor? doctor = await _unitOfWork.DoctorRepository.GetDoctorByUserIdAsync(callerUserId);
                if (doctor is null)
                {
                    _logger.LogWarning("Doctor with id {Id} not found", callerUserId);
                    throw new EntityNotFoundException("Doctor", ErrorMessages.NotFound);
                }

                if (dto.Username != doctor.User.Username)
                {
                    User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(dto.Username!);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("Doctor with username {Username} already exists", dto.Username);
                        throw new EntityAlreadyExistsException("Doctor", ErrorMessages.AlreadyExists);
                    }
                }

                if (dto.Email != doctor.User.Email)
                {
                    User? existingEmail = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email!);
                    if (existingEmail != null)
                    {
                        _logger.LogWarning("Employee with email {Email} already exists", dto.Email);
                        throw new EntityAlreadyExistsException("Email", ErrorMessages.AlreadyExists);
                    }
                }

                _mapper.Map(dto, doctor.User);
                _mapper.Map(dto, doctor);
                doctor.User.Password = _encryptionUtil.Encrypt(doctor.User.Password!);
                doctor.User.ModifiedAt = DateTime.UtcNow;
                doctor.ModifiedAt = DateTime.UtcNow;

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Doctor {Username} was updated successfully!", doctor.User.Username);
                return _mapper.Map<DoctorReadOnlyDTO>(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor with username {Username}", dto.Username);
                throw;
            }
        }

        public async Task<bool> DeleteDoctorAsync(Guid uuid)
        {
            Doctor? doctor = await _unitOfWork.DoctorRepository.GetByUuidAsync(uuid);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with uuid {Uuid} was not found", uuid);
                throw new EntityNotFoundException("Doctor", ErrorMessages.NotFound);
            }

            doctor.IsDeleted = true;
            doctor.DeletedAt = DateTime.UtcNow;
            doctor.ModifiedAt = DateTime.UtcNow;
            doctor.User.ModifiedAt = DateTime.UtcNow;
            doctor.User.DeletedAt = DateTime.UtcNow;
            doctor.User.IsDeleted = true;

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Doctor with uuid {Uuid} was soft deleted successfully", uuid);
            return true;
        }

        public async Task<DoctorReadOnlyDTO> GetDoctorByUuidAsync(Guid uuid)
        {
            Doctor? doctor = await _unitOfWork.DoctorRepository.GetByUuidAsync(uuid);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with uuid {Uuid} was not found", uuid);
                throw new EntityNotFoundException("Doctor", ErrorMessages.NotFound);
            }

            _logger.LogInformation("Doctor with uuid {Uuid} was fetched successfully!", uuid);
            return _mapper.Map<DoctorReadOnlyDTO>(doctor);
        }

        public async Task<PaginatedResult<DoctorReadOnlyDTO>> GetPaginatedFilteredDoctorsAsync(int pageNumber, int pageSize, 
            DoctorFiltersDTO filters)
        {
            List<Expression<Func<Doctor, bool>>> predicates = [];
            if (!string.IsNullOrEmpty(filters.Username)) predicates.Add(d => d.User.Username == filters.Username);
            if (!string.IsNullOrEmpty(filters.Firstname)) predicates.Add(d => d.User.Firstname == filters.Firstname);
            if (!string.IsNullOrEmpty(filters.Lastname)) predicates.Add(d => d.User.Lastname.Contains(filters.Lastname));
            if (!string.IsNullOrEmpty(filters.Email)) predicates.Add(d => d.User.Email == filters.Email);
            if (!string.IsNullOrEmpty(filters.Specialty)) predicates.Add(d => d.Specialty.Contains(filters.Specialty));

            PaginatedResult<Doctor> result = await _unitOfWork.DoctorRepository.GetPaginatedFilteredDoctorsAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<DoctorReadOnlyDTO>()
            {
                Data = _mapper.Map<List<DoctorReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
            };

            _logger.LogInformation("Retrieved {Count} Doctors", dtoResult.Data.Count);
            return dtoResult;
        }
    }
}
