using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.DTOs
{
    public class RoleDto
    {
        public int RoleId { get; set; }  // Can be 0 or omitted on create
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public int? CompanyId { get; set; }
    }
}
