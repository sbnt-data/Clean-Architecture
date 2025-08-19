using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer;
using ShipJobPortal.Infrastructure.DataHelpers;

namespace ShipJobPortal.Infrastructure.Repositories;

public class OtpRepositories : IOtpRepositories
{
    private readonly IDataAccess_Improved _dataAccess;
    private readonly ILogger<OtpRepositories> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _dbKey;


    public OtpRepositories(IDataAccess_Improved dataAccess, ILogger<OtpRepositories> logger,IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger;
        _dataAccess = dataAccess;
        _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
: "DefaultConnection";
    }

    public async Task<ReturnResult<string>> fn_UserOtpSave(UserOtpCredentialsOtp otp)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_EmailID", otp.Email);
            parameters.Add("@p_OtpCode", otp.OtpCode);
            parameters.Add("@p_ToAddress", otp.ToAddress);

            var dbResult = await _dataAccess.QuerySingleAsync<OtpInsertResult>("usp_insert_otp_test", parameters, _dbKey);

            if (dbResult.ReturnStatus == "success" && dbResult.ErrorCode == "200")
            {
                return new ReturnResult<string>(dbResult.ReturnStatus, dbResult.ErrorCode, "OTP Saved");
            }

            _logger.LogWarning("OTP Save Failed: Status = {Status}, Code = {Code}", dbResult.ReturnStatus, dbResult.ErrorCode);
            return new ReturnResult<string>(dbResult.ReturnStatus, dbResult.ErrorCode, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in fn_UserOtpSave");
            throw;
        }
    }


    public async Task<ReturnResult<string>> fn_UserOtpVerify(UserOtpVerifyModel user)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", user.emailid, DbType.String);
            parameters.Add("@UserOtp", user.otp, DbType.String);
            parameters.Add("@IpAddress", user.IpAddress, DbType.String);

            var dbResult = await _dataAccess.QuerySingleAsync<OtpValidationResult>("usp_validate_otp", parameters, _dbKey);

            if (dbResult.ReturnStatus == "success" && dbResult.ErrorCode == "200")
            {
                return new ReturnResult<string>(dbResult.ReturnStatus, dbResult.ErrorCode, "Email verification success");
            }

            return new ReturnResult<string>(dbResult.ReturnStatus ?? "invalid", dbResult.ErrorCode ?? "INVALID_OTP", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in fn_UserOtpVerify");
            throw;
        }
    }




}