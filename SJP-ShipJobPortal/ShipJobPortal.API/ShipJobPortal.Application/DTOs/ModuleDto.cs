using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.DTOs
{
    public class ModuleDto
    {
        public int ModuleId { get; set; }
        public string? MenuName { get; set; }
        public int MenuParentId { get; set; }
        public string? Description { get; set; }
        public short MenuOrder { get; set; }
        public bool? EnableShortcutModule { get; set; }
        public bool? Active { get; set; }
        public int? CompanyId { get; set; }
        public string? ShortCutPageUrl { get; set; }
    }
}
