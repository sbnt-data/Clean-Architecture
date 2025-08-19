using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.DTOs;

public class UserRoleMapDto
{
    public int RoleId { get; set; }
    public int UserId { get; set; }
    public int CompanyId { get; set; }
}
