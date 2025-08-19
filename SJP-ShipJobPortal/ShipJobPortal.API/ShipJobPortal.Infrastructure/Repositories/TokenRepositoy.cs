using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer;

namespace ShipJobPortal.Infrastructure.Repositories
{
    public class TokenRepositoy:ITokenRepository
    {
        private readonly IDataAccess_Improved _dbHelper; // ✅ UPDATED
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginRepository> _logger;
        private readonly string _dbKey;

        public TokenRepositoy(
    IConfiguration configuration,
    IDataAccess_Improved dbHelper, // ✅ UPDATED
    ILogger<LoginRepository> logger)
        {
            _configuration = configuration;
            _dbHelper = dbHelper;
            _logger = logger;

            _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
                ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
                : "DefaultConnection";
        }
        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Token", token);

                var result = await _dbHelper.QueryAsyncTrans<RefreshToken>("usp_get_refresh_token", parameters, _dbKey);

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

                var result = await _dbHelper.ExecuteScalarAsync("usp_delete_refresh_token", parameters, _dbKey);

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

    }
}
