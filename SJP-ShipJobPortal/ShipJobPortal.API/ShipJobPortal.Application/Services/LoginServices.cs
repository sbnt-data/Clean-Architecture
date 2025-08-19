using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using ShipJobPortal.Application.DTOs;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Application.Validators;
using System.Text.RegularExpressions;
using AutoMapper.Internal;

namespace ShipJobPortal.Application.Services;

public class LoginServices : ILoginService
{
    private readonly ILoginRepository _logInRepository;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<LoginServices> _logger;
    private readonly IMapper _mapper;
    private readonly IDbExceptionLogger _dbExceptionLogger;
    private readonly ITokenService _tokenService;
    private readonly ITokenRepository _tokenRepository;
    public LoginServices(ILoginRepository logInRepository, IConfiguration configuration, IPasswordHasher passwordHasher, ILogger<LoginServices> logger, IMapper mapper, IDbExceptionLogger dbExceptionLogger, ITokenService tokenService, ITokenRepository tokenRepository)
    {
        _logInRepository = logInRepository;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _mapper = mapper;
        _dbExceptionLogger = dbExceptionLogger;
        _tokenService = tokenService;
        _tokenRepository = tokenRepository;
    }
    public async Task<ApiResponse<AuthResponse>> GetUserAsync(UserCredentialsDto user)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(user.Email_Id) || !Regex.IsMatch(user.Email_Id, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return new ApiResponse<AuthResponse>(false, null, "Invalid email format.", ErrorCodes.InvalidEmailFormat);
            }
            if (string.IsNullOrWhiteSpace(user.Email_Id) || string.IsNullOrWhiteSpace(user.Password))
            {
                return new ApiResponse<AuthResponse>(false, null, "Email or password cannot be empty.", ErrorCodes.MissingPassword);
            }

            //string encryptedEmail = _encryptionService.Encrypt(user.Email_Id);
            var (loginInfo, result) = await _logInRepository.GetUserLoginInfoAsync(user.Email_Id);

            if (loginInfo == null)
            {
                return new ApiResponse<AuthResponse>(false, null, "User not found.", ErrorCodes.NotFound);
            }

            if (!_passwordHasher.VerifyPassword(user.Password, loginInfo.PasswordHash))
            {
                return new ApiResponse<AuthResponse>(false, null, "Incorrect password.", ErrorCodes.InvalidPassword);
            }

            //if (loginInfo.TokenExpiresAt.HasValue && loginInfo.TokenExpiresAt.Value < DateTime.UtcNow)
            //{
            //    return new ApiResponse<AuthResponse>(false, null, "Token expired.");
            //}

            string accessToken = _tokenService.GenerateJwtToken(user.Email_Id, loginInfo.Role);
            string refreshToken = _tokenService. GenerateRefreshToken();

            int refreshExpiryDays = Convert.ToInt32(_configuration["TokenSettings:RefreshTokenExpiryDays"]);
            int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);

            await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
            {
                Email_Id = user.Email_Id,
                Token = refreshToken,
                TokenExpiryDate = DateTime.UtcNow.AddDays(refreshExpiryDays)
            });

            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Role = loginInfo.Role,
                Email = user.Email_Id,
                UserId = loginInfo.UserId,
                ExpiresIn = jwtExpiryHours * 3600
            };

            return new ApiResponse<AuthResponse>(true, authResponse, "Login successful.", ErrorCodes.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetUserAsync");
            throw;
        }
    }

    public async Task<ApiResponse<object>> CreateUserAsync(UserApplicantDto dto)
    {
        try
        {
            var user = _mapper.Map<UserApplicantModel>(dto);
          

                    var (isValid, errorMsg) = PasswordPolicyValidator.Validate(
                password: user.Password,
                username: user.Email,              // Or dto.Username if different
                previousPassword: null             // Optional: can fetch from history if needed
            );

                    if (!isValid)
                        return new ApiResponse<object>(false, null, $"Password policy violation: {errorMsg}", ErrorCodes.InvalidPassword);

                    user.Password = _passwordHasher.HashPassword(user.Password);
                


                var result = await _logInRepository.CreateUserAsync(user);
                if (result.ErrorCode == "ERR409")
                    return new ApiResponse<object>(false, null, "Email or Contact Number already exists.", ErrorCodes.Conflict);

                if (result.ReturnStatus == "Seafarer Insert Success" && result.ErrorCode == "ERR200")
                    return new ApiResponse<object>(true, null, "User created successfully.", ErrorCodes.Success);

            if (result.ReturnStatus == "Recruiter Insert Success" && result.ErrorCode == "ERR200")
                return new ApiResponse<object>(true, null, "User created successfully.", ErrorCodes.Success);

            return new ApiResponse<object>(false, null, "Failed to create user.", ErrorCodes.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in CreateUserAsync");
            throw;
        }
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var token = await _tokenRepository.GetRefreshTokenAsync(refreshToken);

            if (token == null)
                return new ApiResponse<AuthResponse>(false, null, "Invalid refresh token.", ErrorCodes.InvalidCredentials);

            if (token.TokenExpiryDate < DateTime.UtcNow)
                return new ApiResponse<AuthResponse>(false, null, "Token expired.", ErrorCodes.InvalidCredentials);

            //var (loginInfo, result) = await _logInRepository.GetUserLoginInfoAsync(token.Email_Id);

            //var userRoleDetails = await _logInRepository.GetUserRoleAsync(token.Email_Id);
            //var userRole = userRoleDetails["RoleName"]?.ToString();
            //int userId = userRoleDetails.ContainsKey("UserID") ? Convert.ToInt32(userRoleDetails["UserID"]) : 0;

            int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);
            int refreshExpiryDays = Convert.ToInt32(_configuration["TokenSettings:RefreshTokenExpiryDays"]);

            var newAccessToken = _tokenService.GenerateJwtToken(token.Email_Id, token.Role);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
            {
                Email_Id = token.Email_Id,
                Token = newRefreshToken,
                TokenExpiryDate = DateTime.UtcNow.AddDays(refreshExpiryDays)
            });

            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Role = token.Role,
                Email = token.Email_Id,
                UserId = token.UserId,
                ExpiresIn = jwtExpiryHours * 3600
            };

            return new ApiResponse<AuthResponse>(true, response, "Login successful.", ErrorCodes.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in RefreshTokenAsync");
            throw;
        }
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        try
        {
            var token = await _tokenRepository.GetRefreshTokenAsync(refreshToken);

            if (token == null)
                return false;

            await _tokenRepository.DeleteRefreshTokenAsync(refreshToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in LogoutAsync for token: {RefreshToken}", refreshToken);
            throw;
        }
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto request)
    {
        try
        {
            //string email = _encryptionService.Encrypt(request.Email);
            var password = _mapper.Map<UserApplicantModel>(request);

            var user = await _logInRepository.GetUserRoleAsync(request.Username);
            if (user == null)
            {
                return new ApiResponse<object>(false, null, "User not found", ErrorCodes.NotFound);
            }

            string encryptedPassword = user.PasswordHash?.ToString();

            bool isCurrentPasswordValid = _passwordHasher.VerifyPassword(request.CurrentPassword, encryptedPassword);
            if (!isCurrentPasswordValid)
            {
                return new ApiResponse<object>(false, null, "Current password is incorrect", ErrorCodes.InvalidPassword);
            }

            var (isValid, errorMsg) = PasswordPolicyValidator.Validate(
                  password: password.Password,
                  username: password.Email,
                  previousPassword: null
            );

            if (!isValid)
                return new ApiResponse<object>(false, null, $"Password policy violation: {errorMsg}", ErrorCodes.InvalidPassword);

            string newHashedPassword = _passwordHasher.HashPassword(request.Password);
            await _logInRepository.ResetPasswordAsync(request.Username, encryptedPassword, newHashedPassword);

            return new ApiResponse<object>(true, null, "Password reset successfully.", ErrorCodes.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in ResetPasswordAsync");
            throw;
        }
    }
    public async Task<ApiResponse<string>> VerifySeamanbook_async()
    {
        return null;
    }
    public async Task<ApiResponse<AuthenticateResult?>> AuthenticateUserAsync_test(UserLoginDto loginDto)
    {
        try
        {
            int moduleFetch = -1;
            var result = new AuthenticateResult();

            // 1. Check if user exists
            var user = await _logInRepository.UserExistsAsync(loginDto.Email);
            if (loginDto.LoginType == "register" && user.Data == null)
            {

                // User doesn't exist - auto-create for Google/LinkedIn
                if (loginDto.LoginType.ToLower() == "register")
                {
                    loginDto.Password = _passwordHasher.HashPassword(loginDto.Password);
                }

                var userdata = _mapper.Map<UserApplicantModel>(loginDto);
                userdata.loginType = "Portal";
                var createResult = await _logInRepository.CreateUserAsync(userdata);
                var userCreationData = createResult.Data as UserCreationResult;

                if (userCreationData != null)
                {
                    result.ResumeStatus = userCreationData.Status;
                    result.UserId = userCreationData.UserId;
                }

                if (createResult.ReturnStatus == "Seafarer Insert Success")
                {
                    moduleFetch = 1;
                    result.Email = loginDto.Email;
                    result.Role = loginDto.Role;
                    result.LoginType = userdata.loginType;
                    moduleFetch = 1;
                    var jwtToken = _tokenService.GenerateJwtToken(result.Email, loginDto.Role); // custom method

                    var refreshToken = _tokenService.GenerateRefreshToken();

                    await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
                    {
                        Email_Id = result.Email,
                        Token = refreshToken,
                        TokenExpiryDate = DateTime.UtcNow.AddDays(7), // or from config
                        Role = result.Role,
                        UserId = result.UserId
                    });
                    result.AccessToken = jwtToken;
                    result.RefreshToken = refreshToken;
                }
                else
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "User creation failed.", ErrorCodes.InternalServerError);
                }
            }

            else if (user.Data == null && (loginDto.LoginType.ToLower() == "google" || loginDto.LoginType.ToLower() == "linkedin"))
            {
                var userdata = _mapper.Map<UserApplicantModel>(loginDto);
                var createResult = await _logInRepository.CreateUserAsync(userdata);
                var userCreationData = createResult.Data as UserCreationResult;

                if (userCreationData != null)
                {
                    result.ResumeStatus = userCreationData.Status;
                    result.UserId = userCreationData.UserId;
                }

                if (createResult.ReturnStatus == "Seafarer Insert Success")
                {
                    moduleFetch = 1;
                    result.Email = loginDto.Email;
                    result.Role = loginDto.Role;
                    result.LoginType = loginDto.LoginType;
                    moduleFetch = 1;
                    var jwtToken = _tokenService.GenerateJwtToken(result.Email, loginDto.Role); // custom method
                    var refreshToken = _tokenService.GenerateRefreshToken();

                    await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
                    {
                        Email_Id = result.Email,
                        Token = refreshToken,
                        TokenExpiryDate = DateTime.UtcNow.AddDays(7), // or from config
                        Role = result.Role,
                        UserId = result.UserId
                    });
                    result.AccessToken = jwtToken;
                    result.RefreshToken = refreshToken;
                }
                else
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "User creation failed.", ErrorCodes.InternalServerError);
                }
            }

            else
            {
                // 2. Existing user - verify password if portal
                var getUserResult = await _logInRepository.ExistingUserDetails(loginDto.Email);
                var getUser = getUserResult.Data;

                if (getUser == null)
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "User not verified", ErrorCodes.NotFound);
                }

                if (loginDto.LoginType.ToLower() == "portal" || loginDto.LoginType.ToLower() == "register")
                {
                    if (!_passwordHasher.VerifyPassword(loginDto.Password, getUser.PasswordHash))
                    {
                        return new ApiResponse<AuthenticateResult>(false, null, "Incorrect password.", ErrorCodes.InvalidPassword);
                    }
                }
                result.ResumeStatus = getUser.ResumeStatus;
                if (getUser.RoleName.ToLower() == "recruiter")
                {
                    result.ResumeStatus = "recruiter";

                }
                result.UserId = getUser.UserId ?? 0;
                result.Email = getUser.Username;
                result.Role = getUser.RoleName;
                if (loginDto.Role.ToLower() != getUser.RoleName.ToLower())
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "Invalid role for this login", ErrorCodes.InvalidCredentials);
                }
                result.LoginType = loginDto.LoginType;
                moduleFetch = 1;
                var jwtToken = _tokenService.GenerateJwtToken(result.Email, loginDto.Role); // custom method
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
                {
                    Email_Id = result.Email,
                    Token = refreshToken,
                    TokenExpiryDate = DateTime.UtcNow.AddDays(7), // or from config
                    Role = result.Role,
                    UserId = result.UserId
                });
                result.AccessToken = jwtToken;
                result.RefreshToken = refreshToken;

            }

            // 3. Fetch module privileges
            if (moduleFetch == 1)
            {
                var moduleResult = await _logInRepository.GetModulePrivilegesByRoleAsync(loginDto.Role);

                if (moduleResult.Data == null || moduleResult.Data.Count == 0)
                {
                    return new ApiResponse<AuthenticateResult>(false, result, "No modules assigned to this role", ErrorCodes.NotFound);
                }

                result.RoleModulePrevilleages = _mapper.Map<List<ModulePrevilleages>>(moduleResult.Data);

                return new ApiResponse<AuthenticateResult>(true, result, "Login successful", ErrorCodes.Success);
            }

            return new ApiResponse<AuthenticateResult>(false, null, "Authentication failed", ErrorCodes.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in AuthenticateUserAsync");
            throw;
        }
    }

    /// <summary>
    /// for login
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    public async Task<ApiResponse<AuthenticateResult?>> AuthenticateUserAsync(UserLoginDto loginDto)
    {
        try
        {
            int moduleFetch = -1;
            var result = new AuthenticateResult();

            // 1. Check if user exists
            var user = await _logInRepository.UserExistsAsync(loginDto.Email);
            if (user.Data != null)
            { 
                // 2. Existing user - verify password if portal
                var getUserResult = await _logInRepository.ExistingUserDetails(loginDto.Email);
                var getUser = getUserResult.Data;

                if (getUser == null)
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "User not found", ErrorCodes.NotFound);
                }

                if (loginDto.LoginType.ToLower() == "portal")// || loginDto.LoginType.ToLower() == "register")
                {
                    if (!_passwordHasher.VerifyPassword(loginDto.Password, getUser.PasswordHash))
                    {
                        return new ApiResponse<AuthenticateResult>(false, null, "Incorrect password.", ErrorCodes.InvalidPassword);
                    }
                }
                result.ResumeStatus = getUser.ResumeStatus;
                if (getUser.RoleName.ToLower() == "recruiter")
                {
                    result.ResumeStatus = "recruiter";

                }
                result.UserId = getUser.UserId ?? 0;
                result.Email = getUser.Username;
                result.Role = getUser.RoleName;
                if (loginDto.Role.ToLower() != getUser.RoleName.ToLower())
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "Invalid role for this login", ErrorCodes.InvalidCredentials);
                }
                result.LoginType = loginDto.LoginType;
                moduleFetch = 1;
                var refreshToken = getUser.RefreshToken;

                var jwtToken = _tokenService.GenerateJwtToken(result.Email, loginDto.Role); // custom method
                if (getUser.RefreshToken == null)
                {
                    refreshToken = _tokenService.GenerateRefreshToken();
                    await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
                    {
                        Email_Id = result.Email,
                        Token = refreshToken,
                        TokenExpiryDate = DateTime.UtcNow.AddDays(7), // or from config
                        Role = result.Role,
                        UserId = result.UserId
                    });
                }


                result.AccessToken = jwtToken;
                result.RefreshToken = refreshToken;

            }

            // 3. Fetch module privileges
            if (moduleFetch == 1)
            {
                var moduleResult = await _logInRepository.GetModulePrivilegesByRoleAsync(loginDto.Role);

                if (moduleResult.Data == null || moduleResult.Data.Count == 0)
                {
                    return new ApiResponse<AuthenticateResult>(false, result, "No modules assigned to this role", ErrorCodes.NotFound);
                }

                result.RoleModulePrevilleages = _mapper.Map<List<ModulePrevilleages>>(moduleResult.Data);

                return new ApiResponse<AuthenticateResult>(true, result, "Login successful", ErrorCodes.Success);
            }

            return new ApiResponse<AuthenticateResult>(false, null, "Authentication failed", ErrorCodes.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in AuthenticateUserAsync");
            throw;
        }
    }
    
    
    /// <summary>
    /// for register
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    public async Task<ApiResponse<AuthenticateResult>> RegisterAsyns(UserLoginDto loginDto)
    {
        try
        {
            int moduleFetch = -1;
            var result = new AuthenticateResult();

            // 1. Check if user exists
            var user = await _logInRepository.UserExistsAsync(loginDto.Email);
            if (loginDto.LoginType == "register" && user.Data == null)
            {

                // User doesn't exist - auto-create for Google/LinkedIn
                if (loginDto.LoginType.ToLower() == "register")
                {
                    loginDto.Password = _passwordHasher.HashPassword(loginDto.Password);
                }

                var userdata = _mapper.Map<UserApplicantModel>(loginDto);
                userdata.loginType = "Portal";
                var createResult = await _logInRepository.CreateUserAsync(userdata);
                var userCreationData = createResult.Data as UserCreationResult;

                if (userCreationData != null)
                {
                    result.ResumeStatus = userCreationData.Status;
                    result.UserId = userCreationData.UserId;
                }

                if (createResult.ReturnStatus == "Seafarer Insert Success")
                {
                    moduleFetch = 1;
                    result.Email = loginDto.Email;
                    result.Role = loginDto.Role;
                    result.LoginType = userdata.loginType;
                    moduleFetch = 1;
                    var jwtToken = _tokenService.GenerateJwtToken(result.Email, loginDto.Role); // custom method
                    var refreshToken = _tokenService.GenerateRefreshToken();

                    await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
                    {
                        Email_Id = result.Email,
                        Token = refreshToken,
                        TokenExpiryDate = DateTime.UtcNow.AddDays(7), // or from config
                        Role = result.Role,
                        UserId = result.UserId
                    });
                    result.AccessToken = jwtToken;
                    result.RefreshToken = refreshToken;
                }
                else
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "User creation failed.", ErrorCodes.InternalServerError);
                }
            }
            if (moduleFetch == 1)
            {
                var moduleResult = await _logInRepository.GetModulePrivilegesByRoleAsync(loginDto.Role);

                if (moduleResult.Data == null || moduleResult.Data.Count == 0)
                {
                    return new ApiResponse<AuthenticateResult>(false, result, "No modules assigned to this role", ErrorCodes.NotFound);
                }

                result.RoleModulePrevilleages = _mapper.Map<List<ModulePrevilleages>>(moduleResult.Data);

                return new ApiResponse<AuthenticateResult>(true, result, "Login successful", ErrorCodes.Success);
            }

            return new ApiResponse<AuthenticateResult>(false, null, "Authentication failed", ErrorCodes.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in RegisterAsyns");
            throw;
        }

    }
    
    
    /// <summary>
    /// for Oauth users
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    public async Task<ApiResponse<AuthenticateResult>>OauthuserAsync(UserLoginDto loginDto)
    {
        try
        {
            int moduleFetch = -1;
            var result = new AuthenticateResult();

            // 1. Check if user exists
            var user = await _logInRepository.UserExistsAsync(loginDto.Email);
            if (user.Data == null)
            {
                var userdata = _mapper.Map<UserApplicantModel>(loginDto);
                var createResult = await _logInRepository.CreateUserAsync(userdata);
                var userCreationData = createResult.Data as UserCreationResult;

                if (userCreationData != null)
                {
                    result.ResumeStatus = userCreationData.Status;
                    result.UserId = userCreationData.UserId;
                }

                if (createResult.ReturnStatus == "Seafarer Insert Success")
                {
                    moduleFetch = 1;
                    result.Email = loginDto.Email;
                    result.Role = loginDto.Role;
                    result.LoginType = loginDto.LoginType;
                    moduleFetch = 1;
                    var jwtToken = _tokenService.GenerateJwtToken(result.Email, loginDto.Role); // custom method
                    var refreshToken = _tokenService.GenerateRefreshToken();

                    await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
                    {
                        Email_Id = result.Email,
                        Token = refreshToken,
                        TokenExpiryDate = DateTime.UtcNow.AddDays(7), // or from config
                        Role = result.Role,
                        UserId = result.UserId
                    });
                    result.AccessToken = jwtToken;
                    result.RefreshToken = refreshToken;
                }
                else
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "User creation failed.", ErrorCodes.InternalServerError);
                }
            }
            else
            {
                var getUserResult = await _logInRepository.ExistingUserDetails(loginDto.Email);
                var getUser = getUserResult.Data;

                if (getUser == null)
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "User not found", ErrorCodes.NotFound);
                }

                if (loginDto.LoginType.ToLower() == "portal" || loginDto.LoginType.ToLower() == "register")
                {
                    if (!_passwordHasher.VerifyPassword(loginDto.Password, getUser.PasswordHash))
                    {
                        return new ApiResponse<AuthenticateResult>(false, null, "Incorrect password.", ErrorCodes.InvalidPassword);
                    }
                }
                result.ResumeStatus = getUser.ResumeStatus;
                if (getUser.RoleName.ToLower() == "recruiter")
                {
                    result.ResumeStatus = "recruiter";

                }
                result.UserId = getUser.UserId ?? 0;
                result.Email = getUser.Username;
                result.Role = getUser.RoleName;
                if (loginDto.Role.ToLower() != getUser.RoleName.ToLower())
                {
                    return new ApiResponse<AuthenticateResult>(false, null, "Invalid role for this login", ErrorCodes.InvalidCredentials);
                }
                result.LoginType = loginDto.LoginType;

                var refreshToken = getUser.RefreshToken;

                var jwtToken = _tokenService.GenerateJwtToken(result.Email, loginDto.Role); // custom method
                if (getUser.RefreshToken == null)
                {
                    refreshToken = _tokenService.GenerateRefreshToken();
                    await _tokenRepository.UpdateRefreshTokenAsync(new RefreshToken
                    {
                        Email_Id = result.Email,
                        Token = refreshToken,
                        TokenExpiryDate = DateTime.UtcNow.AddDays(7), // or from config
                        Role = result.Role,
                        UserId = result.UserId
                    });
                }
                moduleFetch = 1;
                result.AccessToken = jwtToken;
                result.RefreshToken = refreshToken;

            }
            if (moduleFetch == 1)
            {
                var moduleResult = await _logInRepository.GetModulePrivilegesByRoleAsync(loginDto.Role);

                if (moduleResult.Data == null || moduleResult.Data.Count == 0)
                {
                    return new ApiResponse<AuthenticateResult>(false, result, "No modules assigned to this role", ErrorCodes.NotFound);
                }

                result.RoleModulePrevilleages = _mapper.Map<List<ModulePrevilleages>>(moduleResult.Data);

                return new ApiResponse<AuthenticateResult>(true, result, "Login successful", ErrorCodes.Success);
            }

            return new ApiResponse<AuthenticateResult>(false, null, "Authentication failed", ErrorCodes.InternalServerError);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in AuthenticateUserAsync");
            throw;
        }
    }
}