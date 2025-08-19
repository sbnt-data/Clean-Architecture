using System.ComponentModel.DataAnnotations;

namespace ShipJobPortal.Application.DTOs;





public class ModulePrevilleages
{
    public string? RoleName {get; set;}
    public string? ModuleID { get; set; }
    public string? ModuleName { get; set; }
    public string? ModulePath { get; set; }
    public int? SortOrder { get; set; }
    public int? ViewAllowed { get; set; }
    public int? AddAllowed { get; set; }
    public int? EditAllowed { get; set; }
    public int? DeleteAllowed { get; set; }
    public int? PrintAllowed { get; set; }
    public int? EmailAllowed { get; set; }
    public int? Hide { get; set; }
    public int? ModuleIsActive { get; set; }
    public int? MappingIsActive {  get; set; }
}