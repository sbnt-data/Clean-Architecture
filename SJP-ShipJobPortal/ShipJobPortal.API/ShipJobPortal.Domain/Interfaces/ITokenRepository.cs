using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces
{
    public interface ITokenRepository
    {
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken token);
        Task DeleteRefreshTokenAsync(string token);

    }
}
