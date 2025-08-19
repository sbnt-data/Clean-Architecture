using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Application.DTOs;

namespace ShipJobPortal.Application.IServices;

public interface IRoleService
{
    Task<ApiResponse<string>> CreateRoleAsync(RoleDto roleDto, string username);
    Task<ApiResponse<IEnumerable<RoleDto>>> GetAllRolesAsync();
    Task<ApiResponse<string>> SaveUserRoleMappingAsync(UserRoleMapDto userRoleMapDto, string username);
    Task<ApiResponse<List<UserRoleMapViewDto>>> GetUserRoleMappingAsync();

}
