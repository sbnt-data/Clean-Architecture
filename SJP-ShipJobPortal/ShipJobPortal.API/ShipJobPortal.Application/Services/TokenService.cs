using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Application.Services;

public class TokenService:ITokenService
{
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<TokenService> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;


    public TokenService(ITokenRepository tokenRepository, ILogger<TokenService> logger, IMapper mapper, IConfiguration configuration)
    {
        _tokenRepository = tokenRepository;
        _logger = logger;
        _mapper = mapper;
        _configuration = configuration;
    }


    public string GenerateJwtToken(string username, string userRole)
    {
        try
        {
            string tokenKey = _configuration["TokenSettings:TokenKey"];
            string issuer = _configuration["TokenSettings:Issuer"];
            string audience = _configuration["TokenSettings:Audience"];
            int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);

            if (string.IsNullOrWhiteSpace(tokenKey) || tokenKey.Length < 16)
                throw new Exception("Token key must be at least 16 characters long.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
    new Claim(ClaimTypes.Email, username),
    new Claim(ClaimTypes.Role, userRole)
};

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(jwtExpiryHours),
                signingCredentials: creds
            );
            //var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in GenerateJwtToken");
            throw;
        }
    }


    public string GenerateRefreshToken()
    {
        try
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
        catch (Exception ex)
        {
            throw;
        }
    }


}
