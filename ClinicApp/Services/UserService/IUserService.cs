using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;
using ClinicApp.Models;
using System.Security.Claims;

namespace ClinicApp.Services.UserService.UserService
{
    public interface IUserService
    {
        Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<UserReadOnlyDTO> UpdateUserAsync(UserUpdateDTO dto, int callerUserId);
        Task<bool> DeleteUserAsync(Guid uuid);
        Task<UserReadOnlyDTO> GetUserByUuidAsync(Guid uuid);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedFilteredUsersAsync(int pageNumber, int pageSize, UserFiltersDTO filters);
        ClaimsPrincipal CreateClaimsPrincipal(User user);
    }
}
