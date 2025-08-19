using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.DTOs
{
    public class RoleModuleMapDto
    {
        public int? Id { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool Hide { get; set; }
        public bool ViewAllowed { get; set; }
        public bool AddAllowed { get; set; }
        public bool EditAllowed { get; set; }
        public bool DeleteAllowed { get; set; }
        public bool PrintAllowed { get; set; }
        public bool EmailAllowed { get; set; }
        public bool Active { get; set; }
        //public int CompanyId { get; set; }
    }
}
