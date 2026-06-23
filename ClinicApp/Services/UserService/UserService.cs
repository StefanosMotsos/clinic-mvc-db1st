using AutoMapper;
using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;
using ClinicApp.Exceptions;
using ClinicApp.Models;
using ClinicApp.Repositories;
using ClinicApp.Resources;
using ClinicApp.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq.Expressions;
using System.Runtime.Versioning;
using System.Security.Claims;

namespace ClinicApp.Services.UserService.UserService
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IEncryptionUtil encryptionUtil, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _encryptionUtil = encryptionUtil;
            _logger = logger;
        }

        public async Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            try
            {
                User? user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(credentials.Username!);
                if (user == null)
                {
                    _logger.LogWarning("User with username {Username} not found", credentials.Username);
                    throw new EntityNotFoundException("User", ErrorMessages.NotFound);
                }

                if (!_encryptionUtil.isValidPassword(credentials.Password!, user.Password))
                {
                    _logger.LogWarning("Invalid credentials for username {Username}", credentials.Username);
                    throw new EntityNotAuthorizedException("User", ErrorMessages.BadCredentials);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("Authentication failed for username {Username}. {Message}", credentials.Username, ex.Message);
                throw;
            }
        }

        public async Task<UserReadOnlyDTO> SaveStaffAsync(StaffSignupDTO dto)
        {
            try
            {
                User user = _mapper.Map<User>(dto);
                User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(dto.Username!);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with username {Username} already exists", dto.Username);
                    throw new EntityAlreadyExistsException("User", ErrorMessages.AlreadyExists);
                }

                User? existingEmail = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email!);
                if (existingEmail != null)
                {
                    _logger.LogWarning("User with email {Email} already exists", dto.Email);
                    throw new EntityAlreadyExistsException("Email", ErrorMessages.AlreadyExists);
                }

                Role? role = await _unitOfWork.RoleRepository.GetByIdAsync(dto.RoleId!.Value);
                user.Role = role!;
                user.RoleId = role!.Id;
                user.Password = _encryptionUtil.Encrypt(dto.Password!);

                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Doctor with {Username} was registered successfully!", user.Username);
                return _mapper.Map<UserReadOnlyDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user with username {Username}", dto.Username);
                throw;
            }
        }

        public async Task<UserReadOnlyDTO> UpdateUserAsync(UserUpdateDTO dto, int callerUserId)
        {
            try
            {
                User? user = await _unitOfWork.UserRepository.GetByIdAsync(callerUserId);
                if (user is null)
                {
                    _logger.LogWarning("User with id {Id} not found", callerUserId);
                    throw new EntityNotFoundException("User", ErrorMessages.NotFound);
                }

                if (dto.Username != user.Username)
                {
                    User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(dto.Username!);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("User with username {Username} already exists", dto.Username);
                        throw new EntityAlreadyExistsException("User", ErrorMessages.AlreadyExists);
                    }
                }

                if (dto.Email != user.Email)
                {
                    User? existingEmail = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email!);
                    if (existingEmail != null)
                    {
                        _logger.LogWarning("User with email {Email} already exists", dto.Email);
                        throw new EntityAlreadyExistsException("Email", ErrorMessages.AlreadyExists);
                    }
                }

                _mapper.Map(dto, user);
                user.Password = _encryptionUtil.Encrypt(dto.Password!);

                user.ModifiedAt = DateTime.UtcNow;

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("User {Username} was updated successfully!", user.Username);
                return _mapper.Map<UserReadOnlyDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with id {Id}", callerUserId);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid uuid)
        {
            User? user = await _unitOfWork.UserRepository.GetByUuidAsync(uuid);
            if (user is null)
            {
                _logger.LogWarning("User with uuid {Uuid} was not found", uuid);
                throw new EntityNotFoundException("User", ErrorMessages.NotFound);
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("User with uuid {Uuid} was soft deleted successfully", uuid);
            return true;
        }

        public async Task<UserReadOnlyDTO> GetUserByUuidAsync(Guid uuid)
        {
            User? user = await _unitOfWork.UserRepository.GetByUuidAsync(uuid);
            if (user is null)
            {
                _logger.LogWarning("User with uuid {Uuid} was not found", uuid);
                throw new EntityNotFoundException("User", ErrorMessages.NotFound);
            }

            _logger.LogInformation("User with uuid {Uuid} was fetched successfully!", uuid);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedFilteredUsersAsync(int pageNumber, int pageSize, UserFiltersDTO filters)
        {
            List<Expression<Func<User, bool>>> predicates = [];
            if (!string.IsNullOrEmpty(filters.Username)) predicates.Add(u => u.Username == filters.Username);
            if (!string.IsNullOrEmpty(filters.Email)) predicates.Add(u => u.Email == filters.Email);
            if (!string.IsNullOrEmpty(filters.UserRole)) predicates.Add(u => u.Role.Name == filters.UserRole);

            PaginatedResult<User> result = await _unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>
            {
                Data = _mapper.Map<List<UserReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {Count} Users", dtoResult.Data.Count);
            return dtoResult;
        }

        public ClaimsPrincipal CreateClaimsPrincipal(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("uuid", user.Uuid.ToString()),
            };

            foreach (var capability in user.Role.Capabilities)
                claims.Add(new Claim("Capability", capability.Name));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
    }
}

