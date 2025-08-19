using AutoMapper;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<RoleService> _logger;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<ApiResponse<string>> CreateRoleAsync(RoleDto roleDto, string username)
    {
        try
        {
            var model = _mapper.Map<RoleModel>(roleDto);
            model.CreatedBy = username;

            var result = await _roleRepository.InsertRoleAsync(model);

            if (result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                var message = roleDto.RoleId > 0 ? "Role updated successfully" : "Role created successfully";
                return new ApiResponse<string>(true, null, message, ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, $"Failed to insert/update role. Error: {result.ErrorCode}", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in CreateRoleAsync");
            throw;
        }
    }

    public async Task<ApiResponse<IEnumerable<RoleDto>>> GetAllRolesAsync()
    {
        try
        {
            var result = await _roleRepository.GetAllRolesAsync();
            if (result.ReturnStatus != "success" || result.Data == null || !result.Data.Any())
            {
                return new ApiResponse<IEnumerable<RoleDto>>(success: false, data: null, message: "No roles found.", errorCode: ErrorCodes.NotFound);
            }

            var mapped = _mapper.Map<IEnumerable<RoleDto>>(result.Data);
            return new ApiResponse<IEnumerable<RoleDto>>(success: true, data: mapped, message: "Roles fetched successfully", errorCode: ErrorCodes.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAllRolesAsync");
            throw;
        }
    }

    public async Task<ApiResponse<string>> SaveUserRoleMappingAsync(UserRoleMapDto userRoleMapDto, string username)
    {
        try
        {
            var model = _mapper.Map<UserRoleMap>(userRoleMapDto);
            model.CreatedBy = username;

            var result = await _roleRepository.SaveUserRoleMappingAsync(model);

            if (result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                return new ApiResponse<string>(true, null, "User role mapping saved successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, $"Failed to save user role mapping. Error: {result.ErrorCode}", result.ErrorCode);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in SaveUserRoleMappingAsync");
            throw;
        }
    }

    public async Task<ApiResponse<List<UserRoleMapViewDto>>> GetUserRoleMappingAsync()
    {
        try
        {
            var result = await _roleRepository.GetUserRoleMappingAsync();

            if (result.ReturnStatus != "success" || result.Data == null)
                return new ApiResponse<List<UserRoleMapViewDto>>(false, null, "User-role mapping not found", ErrorCodes.NotFound);

            var mappedList = _mapper.Map<List<UserRoleMapViewDto>>(result.Data);

            return new ApiResponse<List<UserRoleMapViewDto>>(true, mappedList, "Mappings retrieved successfully", ErrorCodes.Success);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserRoleMappingAsync");
            throw;
        }
    }

}
