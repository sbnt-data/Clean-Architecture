using ShipJobPortal.Application.DTOs;

namespace ShipJobPortal.Application.IServices;

public interface IModuleService
{
    Task<ApiResponse<object>> CreateModuleAsync(ModuleDto dto, string username);
    Task<ApiResponse<IEnumerable<ModuleDto>>> GetAllMainModulesAsync(string mode);
    Task<ApiResponse<List<RoleModuleMapDto>>> GetRoleModuleMapAsync();
    Task<ApiResponse<object>> SaveRoleModuleMapAsync(RoleModuleMapDto dto, string username);
}
