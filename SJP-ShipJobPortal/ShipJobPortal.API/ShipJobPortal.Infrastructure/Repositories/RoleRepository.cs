using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataHelpers;
using System.Data;

namespace ShipJobPortal.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DataAccess _dbHelper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RoleRepository> _logger;

    public RoleRepository(IConfiguration configuration, DataAccess dbHelper, ILogger<RoleRepository> logger)
    {
        _dbHelper = dbHelper;
        _configuration = configuration;
        _logger = logger;

    }

    public async Task<ReturnResult> InsertRoleAsync(RoleModel model)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", model.RoleId);
            //model.CreatedBy = "niswin";
            parameters.Add("@Name", model.Name);
            parameters.Add("@Description", model.Description);
            parameters.Add("@SortOrder", model.SortOrder);
            parameters.Add("@CompanyId", model.CompanyId);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@ReturnRoleId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = await _dbHelper.QueryAsyncTrans("usp_insert_or_update_role", parameters);

            return new ReturnResult
            {
                ReturnStatus = result?.ReturnStatus ?? "error",
                ErrorCode = result?.ErrorCode ?? "ERR500"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in InsertRoleAsync");
            throw;
        }
    }

    public async Task<ReturnResult<List<RoleModel>>> GetAllRolesAsync()
    {
        try
        {
            var result = await _dbHelper.QueryAsyncTrans("usp_get_role_details", new DynamicParameters());

            var roleList = ((IEnumerable<dynamic>)result.ListResultset)
                              .Select(item => new RoleModel
                              {
                                  RoleId = item.RoleId,
                                  Name = item.RoleName,
                                  Description = item.RoleDescription,
                                  SortOrder = item.SortOrder,
                                  CompanyId = item.CompanyID,
                                  CreatedBy = item.CreatedBy
                              }).ToList();

            return new ReturnResult<List<RoleModel>>(result?.ReturnStatus ?? "error", result?.ErrorCode ?? ErrorCodes.InternalServerError, roleList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in GetAllRolesAsync");
            throw;
        }
    }

    public async Task<ReturnResult> SaveUserRoleMappingAsync(UserRoleMap model)
    {
        try
        {
            var parameters = new DynamicParameters();
            //parameters.Add("@Mode", _configuration["StoredProcedureModes:Insert"]);
            parameters.Add("@UserRoleMappingID", model.UserRoleMappingId);
            parameters.Add("@UserId", model.UserId);
            parameters.Add("@RoleId", model.RoleId);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@StartDate", model.StartDate);
            parameters.Add("@EndDate", model.EndDate);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@ReturnId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = await _dbHelper.QueryAsyncTrans("usp_insert_or_update_user_role_mapping", parameters);

            return new ReturnResult
            {
                ReturnStatus = result?.ReturnStatus ?? "error",
                ErrorCode = result?.ErrorCode ?? "ERR500"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in SaveUserRoleMappingAsync");
            throw;
        }
    }

    public async Task<ReturnResult<List<UserRoleMapView>>> GetUserRoleMappingAsync()
    {
        try
        {
            var parameters = new DynamicParameters();

            var result = await _dbHelper.QueryAsyncTrans("usp_get_user_role_mapping", parameters);

            var list = ((IEnumerable<dynamic>)result.ListResultset)
                .Select(item => new UserRoleMapView
                {
                    UserRoleMappingId = item.UserRoleMappingID,
                    UserId = item.UserID,
                    UserName = item.UserName,
                    RoleId = item.RoleID,
                    RoleName = item.RoleName,
                    IsActive = item.IsActive,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate
                })
                .ToList();

            if (!list.Any())
            {
                return new ReturnResult<List<UserRoleMapView>>("error", ErrorCodes.NotFound, null);
            }

            return new ReturnResult<List<UserRoleMapView>>("success", ErrorCodes.Success, list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserRoleMappingAsync");
            throw;
        }
    }

}
