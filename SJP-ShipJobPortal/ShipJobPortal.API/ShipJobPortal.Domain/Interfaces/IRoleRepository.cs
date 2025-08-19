using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface IRoleRepository
{
    Task<ReturnResult> InsertRoleAsync(RoleModel model);
    Task<ReturnResult<List<RoleModel>>> GetAllRolesAsync();
    Task<ReturnResult> SaveUserRoleMappingAsync(UserRoleMap model);
    Task<ReturnResult<List<UserRoleMapView>>> GetUserRoleMappingAsync();

}
