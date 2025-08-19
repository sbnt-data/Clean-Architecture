using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Application.IServices;

public interface ILoginService
{
    Task<ApiResponse<AuthResponse>> GetUserAsync(UserCredentialsDto user);
    Task<ApiResponse<object>> CreateUserAsync(UserApplicantDto dto);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string refreshToken);
    Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto request);

    Task<ApiResponse<AuthenticateResult?>> AuthenticateUserAsync_test(UserLoginDto loginDto);
    Task<ApiResponse<AuthenticateResult?>> AuthenticateUserAsync(UserLoginDto loginDto);

    Task<ApiResponse<AuthenticateResult>> RegisterAsyns(UserLoginDto loginDto);
    Task<ApiResponse<AuthenticateResult>> OauthuserAsync(UserLoginDto loginDto);
    Task<ApiResponse<string>> VerifySeamanbook_async();
}
