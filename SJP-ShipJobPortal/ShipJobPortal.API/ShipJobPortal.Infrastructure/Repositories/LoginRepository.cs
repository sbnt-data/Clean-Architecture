using System.Data;
using Dapper;
using Newtonsoft.Json;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer; // ✅ NEW NAMESPACE
using ShipJobPortal.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace ShipJobPortal.Infrastructure.Repositories;

public class LoginRepository : ILoginRepository
{
    private readonly IDataAccess_Improved _dbHelper; // ✅ UPDATED
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginRepository> _logger;
    private readonly string _dbKey;


    public LoginRepository(
        IConfiguration configuration,
        IPasswordHasher passwordHasher,
        IDataAccess_Improved dbHelper, // ✅ UPDATED
        ILogger<LoginRepository> logger)
    {
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _dbHelper = dbHelper;
        _logger = logger;

        _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
            ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
            : "DefaultConnection";
    }

    public async Task<ResetPasswordDataModel> GetUserRoleAsync(string username)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Username", username);

            var result = await _dbHelper.QueryAsyncTrans<ResetPasswordDataModel>("usp_get_user_details", parameters,_dbKey);

            if (result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                return result.Data.FirstOrDefault();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetUserRoleAsync");
            throw;
        }

        return null;
    }



    public async Task<ReturnResult> CreateUserAsync(UserApplicantModel user)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            var parameters = new DynamicParameters();
            parameters.Add("@role", user.Role);
            parameters.Add("@loginType", user.loginType);
            parameters.Add("@FirstName", user.FirstName);
            parameters.Add("@MiddleName", user.MiddleName);
            parameters.Add("@Surname", user.Surname);
            parameters.Add("@Title", user.Title);
            parameters.Add("@Designation", user.Designation);
            parameters.Add("@CompanyId", user.CompanyId);
            parameters.Add("@CompanyDetails", user.CompanyDetails);
            parameters.Add("@Email", user.Email);
            parameters.Add("@ContactNumber", user.ContactNumber);
            parameters.Add("@CountryCode", user.CountryCode);
            parameters.Add("@Nationality", user.Nationality);
            parameters.Add("@Address", user.Address);
            parameters.Add("@CityID", user.CityID);
            parameters.Add("@StateID", user.StateID);
            parameters.Add("@CountryID", user.CountryID);
            parameters.Add("@PostalCode", user.PostalCode);
            parameters.Add("@DateOfBirth", user.DateOfBirth);
            parameters.Add("@PasswordHash", user.Password);
            parameters.Add("@userIdoutput", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@status", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

            var result = await _dbHelper.ExecuteScalarAsync("usp_create_user_account", parameters, _dbKey);

            var status = parameters.Get<string>("@status");
            var userId = parameters.Get<int>("@userIdoutput");

            return new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode"),
                Data = new UserCreationResult
                {
                    Status = status,
                    UserId = userId
                }
            };


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in CreateUserAsync");
            throw;
        }
    }



    public async Task ResetPasswordAsync(string email, string oldPassword, string newHashedPassword)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email_Id", email);
            parameters.Add("@OldPassword", oldPassword);
            parameters.Add("@NewPassword", newHashedPassword);

            var result = await _dbHelper.ExecuteScalarAsync("usp_reset_user_password", parameters, _dbKey);

            if (result.ReturnStatus != "success" || result.ErrorCode != "ERR200")
            {
                throw new Exception($"Reset password failed. Error: {result.ErrorCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in {Method}", nameof(ResetPasswordAsync));
            throw;
        }
    }

    public async Task<(LoginUserInfo UserInfo, ReturnResult Result)> GetUserLoginInfoAsync(string Email)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@EmailId", Email);
        parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@ErrorCode", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

        try
        {
            var result = await _dbHelper.QueryAsync<LoginUserInfo>("usp_check_login", parameters,_dbKey);

            var returnResult = new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };

            var user = result.Data.FirstOrDefault();
            return (user, returnResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetUserLoginInfoAsync");
            throw;
        }
    }

    public async Task<(LoginUserInfo UserInfo, ReturnResult Result)> GetUserLoginCheck(string Email)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@EmailId", Email);
        parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@ErrorCode", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

        try
        {
            var result = await _dbHelper.QueryAsync<LoginUserInfo>("usp_check_userlogin", parameters,_dbKey);

            var returnResult = new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };

            var user = result.Data.FirstOrDefault();
            return (user, returnResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetUserLoginInfoAsync");
            throw;
        }
    }

    //paralell
    public async Task<ReturnResult> UserExistsAsync(string email)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email, DbType.String);
            parameters.Add("@Exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            parameters.Add("@ErrorCode", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var result = await _dbHelper.ExecuteScalarAsync("usp_CheckUserExistsByEmailRole", parameters, _dbKey);

            string returnStatus = result.ReturnStatus;
            string errorCode = result.ErrorCode;

            bool? exists = returnStatus == "User Exist"
                ? parameters.Get<bool>("@Exists")
                : null;

            return new ReturnResult
            {
                ReturnStatus = returnStatus,
                ErrorCode = errorCode,
                Data = exists
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in UserExistsAsync");
            throw;
        }
    }




    public async Task<ReturnResult<ExistingUserData>> ExistingUserDetails(string email)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email, DbType.String);

            var result = await _dbHelper.QueryAsyncTrans<ExistingUserData>("usp_get_user_login_details_by_email", parameters,_dbKey);
            var firstOrDefault = result.Data?.FirstOrDefault();

            return new ReturnResult<ExistingUserData>
            {
                ReturnStatus = result.ReturnStatus,
                ErrorCode = result.ErrorCode,
                Data = firstOrDefault
            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in ExistingUserDetails");
            throw;
        }
    }

    public async Task<ReturnResult<List<ModulePrevilleagesModel>>> GetModulePrivilegesByRoleAsync(string userRole)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserRole", userRole, DbType.String);
            parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
            parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

            // Execute stored procedure
            var result = await _dbHelper.QueryAsyncTrans<ModulePrevilleagesModel>(
                "usp_get_modules_by_role", parameters,_dbKey);

            return new ReturnResult<List<ModulePrevilleagesModel>>
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode"),
                Data = result.Data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetModulePrivilegesByRoleAsync");

            throw;
        }
    }


}

/*using System.Data;
using Dapper;
using Newtonsoft.Json;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataHelpers;
using ShipJobPortal.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ShipJobPortal.Infrastructure.Repositories
{
public class LoginRepository : ILoginRepository
{
    private readonly DataAccess _dbHelper;
    //private readonly IEncryptionService _encryptionService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginRepository> _logger;

    public LoginRepository(
        IConfiguration configuration, IPasswordHasher passwordHasher, DataAccess dbHelper, ILogger<LoginRepository> logger)
    {
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _dbHelper = dbHelper;
        _logger = logger;
    }



    public async Task<RefreshToken> GetRefreshTokenAsync(string token)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Token", token);

            var result = await _dbHelper.QueryAsyncTrans<RefreshToken>("usp_get_refresh_token", parameters,_dbKey);

            if (result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                return result.Data.FirstOrDefault();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in GetRefreshTokenAsync");
            throw;
        }

        return null;
    }



    public async Task UpdateRefreshTokenAsync(RefreshToken token)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Token", token.Token);
            parameters.Add("@TokenExpiresAt", token.TokenExpiryDate);
            parameters.Add("@Username", token.Email_Id);

            var result = await _dbHelper.ExecuteScalarAsync("usp_update_refresh_token", parameters, _dbKey);

            if (result.ReturnStatus != "success" || result.ErrorCode != "ERR200")
            {
                throw new Exception($"Failed to update refresh token. ErrorCode: {result.ErrorCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in UpdateRefreshTokenAsync");
            throw;
        }
    }

    public async Task DeleteRefreshTokenAsync(string token)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Token", token);

            var result = await _dbHelper.ExecuteScalarAsync("usp_delete_refresh_token", parameters,_dbKey);

            if (result.ReturnStatus != "success" || result.ErrorCode != "ERR200")
            {
                throw new Exception($"Failed to delete refresh token. ErrorCode: {result.ErrorCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in DeleteRefreshTokenAsync");
            throw;
        }
    }
    public async Task<Dictionary<string, object>> GetUserRoleAsync(string username)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Username", username);

            var result = await _dbHelper.QueryAsyncTrans("User_Details", parameters);

            if (result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                var firstRow = result.ListResultset.FirstOrDefault();
                if (firstRow is IDictionary<string, object> dict)
                {
                    return dict.ToDictionary(k => k.Key, v => v.Value);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetUserRoleAsync");
            throw;
        }

        return null;
    }

    public async Task<ReturnResult> CreateUserAsync(UserApplicantModel user)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.ContactNumber))
            {
                throw new ArgumentException("Email and Contact Number are required.");
            }
            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", user.FirstName);
            parameters.Add("@MiddleName", user.MiddleName);
            parameters.Add("@Surname", user.Surname);
            parameters.Add("@Title", user.Title);
            parameters.Add("@Designation", user.Designation);
            parameters.Add("@CompanyId", user.CompanyId);
            parameters.Add("@CompanyDetails", user.CompanyDetails);
            //parameters.Add("@Website", user.Website);
            parameters.Add("@Email", user.Email);
            parameters.Add("@ContactNumber", user.ContactNumber);
            parameters.Add("@CountryCode", user.CountryCode);
            parameters.Add("@Nationality", user.Nationality);
            parameters.Add("@Address", user.Address);
            parameters.Add("@CityID", user.CityID);
            parameters.Add("@StateID", user.StateID);
            parameters.Add("@CountryID", user.CountryID);
            parameters.Add("@PostalCode", user.PostalCode);
            parameters.Add("@DateOfBirth", user.DateOfBirth);
            parameters.Add("@PasswordHash", user.Password);

            //parameters.Add("@RoleId", user.RoleId);
            //parameters.Add("@UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var usercreate = await _dbHelper.QueryAsyncTrans("usp_create_user_account", parameters);
            var result = new ReturnResult
            {
                ReturnStatus = usercreate.ReturnStatus,
                ErrorCode = usercreate.ErrorCode
            };
            //if (usercreate.ReturnStatus == "success" && usercreate.ErrorCode == "ERR200")
            //{
            //    result.ScalarResult = parameters.Get<int>("@UserId");
            //}
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in CreateUserAsync");
            throw;
        }
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string token)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Token", token);

            var result = await _dbHelper.QueryAsyncTrans("Get_RefreshToken", parameters);

            if (result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                var dynamicResult = result.ListResultset.FirstOrDefault();
                if (dynamicResult != null)
                {
                    var json = JsonConvert.SerializeObject(dynamicResult);
                    return JsonConvert.DeserializeObject<RefreshToken>(json);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in GetRefreshTokenAsync");
            throw;
        }

        return null;
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken token)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Token", token.Token);
            parameters.Add("@TokenExpiresAt", token.TokenExpiryDate);
            parameters.Add("@Username", token.Email_Id);

            var result = await _dbHelper.QueryAsyncTrans("usp_update_refresh_token", parameters);

            if (result.ReturnStatus != "success" || result.ErrorCode != "ERR200")
            {
                throw new Exception($"Failed to update refresh token. ErrorCode: {result.ErrorCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in UpdateRefreshTokenAsync");
            throw;
        }
    }

    public async Task DeleteRefreshTokenAsync(string token)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Token", token);

            var result = await _dbHelper.QueryAsyncTrans("Delete_RefreshToken", parameters);

            if (result.ReturnStatus != "success" || result.ErrorCode != "ERR200")
            {
                throw new Exception($"Failed to delete refresh token. ErrorCode: {result.ErrorCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in DeleteRefreshTokenAsync");
            throw;
        }
    }

    public async Task ResetPasswordAsync(string email, string oldPassword, string newHashedPassword)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email_Id", email);
            parameters.Add("@OldPassword", oldPassword);
            parameters.Add("@NewPassword", newHashedPassword);

            var result = await _dbHelper.QueryAsyncTrans("Reset_UserPassword", parameters);

            if (result.ReturnStatus != "success" || result.ErrorCode != "ERR200")
            {
                throw new Exception($"Reset password failed. Error: {result.ErrorCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in {Method}", nameof(ResetPasswordAsync));
            throw;
        }
    }

    public async Task<(LoginUserInfo UserInfo, ReturnResult Result)> GetUserLoginInfoAsync(string Email)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@EmailId", Email);
        parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@ErrorCode", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

        try
        {
            var result = await _dbHelper.QueryAsyncTrans("usp_check_login", parameters);

            var returnResult = new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };

            if (returnResult.ReturnStatus == "success" &&
                returnResult.ErrorCode == "ERR200" &&
                result.ListResultset.Any())
            {
                var row = result.ListResultset.First();

                var userInfo = new LoginUserInfo
                {
                    PasswordHash = row?.PasswordHash,
                    TokenExpiresAt = row?.TokenExpiresAt,
                    UserId = row?.UserId,
                    Role = row?.Role
                };

                return (userInfo, returnResult);
            }

            return (null, returnResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetUserLoginInfoAsync");
            throw;
        }
    }


}

}
*/