using AutoMapper;
using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;
using ClinicApp.Exceptions;
using ClinicApp.Models;
using ClinicApp.Repositories.UoW;
using ClinicApp.Security;
using System.Linq.Expressions;
using System.Runtime.Versioning;

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
        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            User? user = null;

            try 
            {
                user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(credentials.Username!);
                if (user == null || !_encryptionUtil.isValidPassword(credentials.Password!, user.Password))
                {
                    throw new EntityNotAuthorizedException("User", Resources.ErrorMessages.BadCredentials);
                };

                _logger.LogInformation("User with username {Username} verified for login", credentials.Username!);
            } catch (EntityNotAuthorizedException e)
            {
                _logger.LogError("Authentication failed for username {Username}. {Message}", credentials.Username, e.Message);
            }
            return user;
        }

        public async Task<UserReadOnlyDTO> GetUserByUsernameAsync(string username)
        {
            try
            {
                User? user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    throw new EntityNotFoundException("User", "User with username: " + username + " not found");
                }
                _logger.LogInformation("User found: {Username}", username);
                return _mapper.Map<UserReadOnlyDTO>(user);
            } catch (EntityNotFoundException e)
            {
                _logger.LogError("Error retrieving user by username: {Username}. {Message}",
                    username, e.Message);
                throw;
            }
        }

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(
            int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO)
        {
            List<User> users = [];
            List<Expression<Func<User, bool>>> predicates = [];

            if (!string.IsNullOrEmpty(userFiltersDTO.Username))
            {
                predicates.Add(u => u.Username == userFiltersDTO.Username);
            }

            if (!string.IsNullOrEmpty(userFiltersDTO.Email))
            {
                predicates.Add(u => u.Email == userFiltersDTO.Email);
            }

            if (!string.IsNullOrEmpty(userFiltersDTO.UserRole))
            {
                predicates.Add(u => u.Role.Name == userFiltersDTO.UserRole);
            }

            var result = await _unitOfWork.UserRepository.GetUserAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = _mapper.Map<List<UserReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            _logger.LogInformation("Retrieved {Count} users", dtoResult.Data.Count);
            return dtoResult;
        }
    }
}
