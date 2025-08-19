using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class RefreshToken
{
    public string Email_Id { get; set; }
    public string Token { get; set; }
    public DateTime TokenExpiryDate { get; set; }
    public string Role { get; set; }
    public int UserId { get; set; }
}

public class TokenRequest
{
    public string RefreshToken { get; set; }
}
