using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class RoleModel
{
    public int RoleId {get;set;}
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public int? CompanyId { get; set; }
    [JsonIgnore]
    public string? CreatedBy { get; set; }
}
