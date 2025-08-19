using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface ILoginRepository
{
    Task<ResetPasswordDataModel> GetUserRoleAsync(string username);
    Task<ReturnResult> CreateUserAsync(UserApplicantModel model);

    Task ResetPasswordAsync(string email, string oldPassword, string newHashedPassword);
    Task<(LoginUserInfo UserInfo, ReturnResult Result)> GetUserLoginInfoAsync(string Email);
    Task<(LoginUserInfo UserInfo, ReturnResult Result)> GetUserLoginCheck(string Email);
    //parallel

    Task<ReturnResult> UserExistsAsync(string email);
    Task<ReturnResult<ExistingUserData>> ExistingUserDetails(string email);
    Task<ReturnResult<List<ModulePrevilleagesModel>>> GetModulePrivilegesByRoleAsync(string userRole);
    
    
    //Task<RefreshToken> GetRefreshTokenAsync(string token);
    //Task UpdateRefreshTokenAsync(RefreshToken token);
    //Task DeleteRefreshTokenAsync(string token);
}