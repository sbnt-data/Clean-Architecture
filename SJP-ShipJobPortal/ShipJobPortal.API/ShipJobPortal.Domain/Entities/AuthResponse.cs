using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
    public int UserId { get; set; }
    public int ExpiresIn { get; set; }
}
