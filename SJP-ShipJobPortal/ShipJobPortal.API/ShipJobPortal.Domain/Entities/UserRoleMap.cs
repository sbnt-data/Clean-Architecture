using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class UserRoleMap
{
    public int UserRoleMappingId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CreatedBy { get; set; }
}
