using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Application.DTOs;

namespace ShipJobPortal.Application.IServices;

public interface IMatchingService
{
    Task<ApiResponse<string>> SendCandidatestoShipHire(CandidateListtoShiphireDto model);

    Task<ApiResponse<MatchingCandidateViewDto>> GetMatchingCandidatesAsync(int JobId,int userId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0);

}
