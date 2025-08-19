using AutoMapper;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.Services;

public class ModuleService : IModuleService
{
    private readonly IModuleRepository _moduleRepository;
    private readonly ILogger<ModuleService> _logger;
    private readonly IMapper _mapper;
    //private readonly IDbExceptionLogger _dbExceptionLogger;

    public ModuleService(IModuleRepository moduleRepository, ILogger<ModuleService> logger, IMapper mapper)
    {
        _moduleRepository = moduleRepository;
        _logger = logger;
        _mapper = mapper;
        //_dbExceptionLogger = dbExceptionLogger;
    }
    public async Task<ApiResponse<object>> CreateModuleAsync(ModuleDto dto, string username)
    {
        try
        {
            var model = _mapper.Map<ModuleModel>(dto);
            model.CreatedBy = username;
            //model.ShortCutPageUrl ??= $"/{dto.MenuName?.Replace(" ", "").ToLower()}/index";

            var module = await _moduleRepository.CreateModuleAsync(model);

            if (module.ReturnStatus == "success" && module.ErrorCode == "ERR200")
            {
                if (dto.ModuleId > 0)
                    return new ApiResponse<object>(true, null, "Updated successfully", ErrorCodes.Success);
                else
                    return new ApiResponse<object>(true, null, "Created successfully", ErrorCodes.Success);
            }

            return new ApiResponse<object>(false, 0, "Failed to create module", ErrorCodes.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateModuleAsync", ErrorCodes.InternalServerError);
            throw;
        }
    }

    public async Task<ApiResponse<IEnumerable<ModuleDto>>> GetAllMainModulesAsync(string mode)
    {
        try
        {
            var result = await _moduleRepository.GetAllMainModulesAsync(mode);

            //return _mapper.Map< ApiResponse<IEnumerable<ModuleDto>>>(result);
            var mapped = _mapper.Map<IEnumerable<ModuleDto>>(result.Data);
            return new ApiResponse<IEnumerable<ModuleDto>>(true, mapped, "Modules retrieved successfully", result.ErrorCode);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllMainModulesAsync", ErrorCodes.InternalServerError);
            throw;
        }
    }

    public async Task<ApiResponse<List<RoleModuleMapDto>>> GetRoleModuleMapAsync()
    {
        try
        {
            var result = await _moduleRepository.GetRoleModuleMapAsync();

            if (result?.Data == null || !result.Data.Any())
            {
                return new ApiResponse<List<RoleModuleMapDto>>(success: false, data: null, message: "No RoleModuleMap data found.", errorCode: ErrorCodes.NotFound);
            }

            var mapped = _mapper.Map<List<RoleModuleMapDto>>(result.Data);

            return new ApiResponse<List<RoleModuleMapDto>>(success: true, data: mapped, message: "RoleModuleMap data retrieved successfully.", errorCode: ErrorCodes.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in GetRoleModuleMapAsync");
            throw;
        }
    }

    public async Task<ApiResponse<object>> SaveRoleModuleMapAsync(RoleModuleMapDto dto, string username)
    {
        try
        {
            var model = _mapper.Map<RoleModuleMapModel>(dto);
            model.CreatedBy = username;

            var result = await _moduleRepository.SaveRoleModuleMapAsync(model);

            if (result != null && result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                return new ApiResponse<object>(true, null, "Role-Module mapping saved successfully.", ErrorCodes.Success);
            }

            return new ApiResponse<object>(false, null, $"Failed to save Role-Module mapping. Error: {result?.ErrorCode ?? "Unknown error"}", ErrorCodes.NotFound);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SaveRoleModuleMapAsync", ErrorCodes.InternalServerError);
            throw;
        }
    }
}