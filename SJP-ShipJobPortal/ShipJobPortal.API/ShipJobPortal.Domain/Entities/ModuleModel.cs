using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class ModuleModel
{
    public int ModuleId { get; set; }
    public int? ParentModuleId { get; set; }
    public string? ModuleName { get; set; }
    public string? ModuleDescription { get; set; }
    public string? ModulePath { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
}
