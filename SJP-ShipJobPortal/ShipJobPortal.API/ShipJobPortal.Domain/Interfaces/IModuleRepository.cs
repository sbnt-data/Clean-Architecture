using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface IModuleRepository
{
    Task<ReturnResult> CreateModuleAsync(ModuleModel model);
    Task<ReturnResult<List<ModuleModel>>> GetAllMainModulesAsync(string mode);
    Task<ReturnResult<List<RoleModuleMapModel>>> GetRoleModuleMapAsync();
    Task<ReturnResult> SaveRoleModuleMapAsync(RoleModuleMapModel model);

}
