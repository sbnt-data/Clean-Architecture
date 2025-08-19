using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataHelpers;

namespace ShipJobPortal.Infrastructure.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly DataAccess _dbHelper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ModuleRepository> _logger;
    public ModuleRepository(IConfiguration configuration, DataAccess dbHelper, ILogger<ModuleRepository> logger)
    {
        _dbHelper = dbHelper;
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<ReturnResult> CreateModuleAsync(ModuleModel model)
    {
        try
        {
            var parameters = new DynamicParameters();
            //parameters.Add("@Mode", _configuration["StoredProcedureModes:Insert"]); // ✅ Add mode
            parameters.Add("@ModuleID", model.ModuleId);
            parameters.Add("@ParentModuleID", model.ParentModuleId);
            parameters.Add("@ModuleName", model.ModuleName);
            parameters.Add("@ModuleDescription", model.ModuleDescription);
            parameters.Add("@ModulePath", model.ModulePath);
            parameters.Add("@SortOrder", model.SortOrder);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy);

            var result = await _dbHelper.QueryAsyncTrans("usp_create_or_update_module", parameters);

            return new ReturnResult
            {
                ReturnStatus = result?.ReturnStatus ?? "error",
                ErrorCode = result?.ErrorCode ?? "ERR500"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in CreateModuleAsync");
            throw;
        }
    }
    public async Task<ReturnResult<List<ModuleModel>>> GetAllMainModulesAsync(string mode)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", mode);

            var result = await _dbHelper.QueryAsyncTrans("usp_get_module_details", parameters);

            var list = result.ListResultset
                .Select(row => new ModuleModel
                {
                    ModuleId = (int)row.ModuleID,
                    ParentModuleId = row.ParentModuleID != null ? (int)row.ParentModuleID : null,
                    ModuleName = (string?)row.ModuleName,
                    ModuleDescription = (string?)row.ModuleDescription,
                    ModulePath = (string?)row.ModulePath,
                    SortOrder = row.SortOrder != null ? (int)row.SortOrder : 0,
                    IsActive = row.IsActive != null && (bool)row.IsActive,
                    CreatedBy = (string?)row.CreatedBy,
                    //ShortCutPageUrl = (string?)row.ShortCutPageUrl
                })
                .ToList();

            return new ReturnResult<List<ModuleModel>>(result?.ReturnStatus ?? "error", result?.ErrorCode ?? "ERR500", list);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAllMainModulesAsync");
            throw;
        }
    }


    public async Task<ReturnResult<List<RoleModuleMapModel>>> GetRoleModuleMapAsync()
    {
        try
        {
            var parameters = new DynamicParameters();
            var result = await _dbHelper.QueryAsyncTrans("usp_get_rolemodule_map", parameters);

            var list = result.ListResultset
                .Select(row => new RoleModuleMapModel
                {
                    Id = row.ModuleRoleMappingID != null ? (int)row.ModuleRoleMappingID : 0,
                    RoleId = row.RoleID != null ? (int)row.RoleID : 0,
                    ModuleId = row.ModuleID != null ? (int)row.ModuleID : 0,
                    Hide = row.Hide != null && (bool)row.Hide,
                    ViewAllowed = row.ViewAllowed != null && (bool)row.ViewAllowed,
                    AddAllowed = row.AddAllowed != null && (bool)row.AddAllowed,
                    EditAllowed = row.EditAllowed != null && (bool)row.EditAllowed,
                    DeleteAllowed = row.DeleteAllowed != null && (bool)row.DeleteAllowed,
                    PrintAllowed = row.PrintAllowed != null && (bool)row.PrintAllowed,
                    EmailAllowed = row.EmailAllowed != null && (bool)row.EmailAllowed
                    //CompanyId = (int)row.CompanyId
                })
                .ToList();

            return new ReturnResult<List<RoleModuleMapModel>>(
         result?.ReturnStatus ?? "error",
         result?.ErrorCode ?? "ERR500",
         list
         );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetRoleModuleMapAsync");
            throw;
        }
    }

    public async Task<ReturnResult> SaveRoleModuleMapAsync(RoleModuleMapModel model)
    {
        try
        {
            // Setup parameters using Dapper's DynamicParameters
            var parameters = new DynamicParameters();
            //parameters.Add("@Mode", _configuration["StoredProcedureModes:Insert"]);
            parameters.Add("@Id", model.Id ?? 0);
            parameters.Add("@RoleId", model.RoleId);
            parameters.Add("@ModuleId", model.ModuleId);
            parameters.Add("@Hide", model.Hide);
            parameters.Add("@ViewAllowed", model.ViewAllowed);
            parameters.Add("@AddAllowed", model.AddAllowed);
            parameters.Add("@EditAllowed", model.EditAllowed);
            parameters.Add("@DeleteAllowed", model.DeleteAllowed);
            parameters.Add("@PrintAllowed", model.PrintAllowed);
            parameters.Add("@EmailAllowed", model.EmailAllowed);
            parameters.Add("@Active", model.Active);
            parameters.Add("@CreatedBy", model.CreatedBy ?? string.Empty);

            // Output parameters to capture result status and error code
            parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
            parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

            await _dbHelper.QueryAsyncTrans("usp_insert_or_update_rolemodule_map", parameters);

            // Retrieve the output parameters from the result (using Dapper's Get method)
            return new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in SaveRoleModuleMapAsync");
            throw;
        }
    }

}