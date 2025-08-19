using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class UserRoleMapView
{
    public int UserRoleMappingId { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
