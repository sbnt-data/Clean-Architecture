using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class LoginUserInfo
{
    public string PasswordHash { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; }
    public string LoginType { get; set; } 
}
