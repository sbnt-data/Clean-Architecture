using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace ShipJobPortal.Application.IServices;

public interface ITokenService
{
    string GenerateRefreshToken();
    string GenerateJwtToken(string username, string userRole);


}
