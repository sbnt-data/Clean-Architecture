using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Application.Services;

public class MatchingService:IMatchingService
{
    private readonly IMatchingRepository _matchRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchingService> _logger;
    private readonly IShiphireRepository _shiphireRepository;

    public MatchingService(IMatchingRepository matchRepository, IMapper mapper, ILogger<MatchingService> logger, IShiphireRepository shiphireRepository)
    {
        _matchRepository = matchRepository;
        _mapper = mapper;
        _logger = logger;
        _shiphireRepository = shiphireRepository;
    }


    public async Task<ApiResponse<string>> SendCandidatestoShipHire(CandidateListtoShiphireDto model)
    {
        try
        {

            var candidatelist = model.candidates;
            foreach (var candidate in candidatelist)
            {

            }
            //var mappedModel = _mapper.Map<JobWishlistModel>(model);

            //var result = await _matchRepository.WishlistJobAsync(mappedModel);

            //if (result.ReturnStatus == "Success" && result.ErrorCode == "ERR200")
            //{
            //    return new ApiResponse<string>
            //    (
            //        true,
            //        result.Data,
            //        "Job added/removed to wishlist",
            //        ErrorCodes.Success
            //    );
            //}
            //else
            //{
            //    return new ApiResponse<string>
            //    (
            //        false,
            //        null,
            //        "Failed to save job application.",
            //        result.ErrorCode
            //    );
            //}
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in SaveJobAsync service method");
            throw;
        }
    }

    public async Task<ApiResponse<MatchingCandidateViewDto>> GetMatchingCandidatesAsync(int JobId,int userId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0)
    {
        try
        {
            var result = await _matchRepository.GetMatchingCandidatesAsync(JobId,userId, pageNumber, pageSize, positionId, vesselTypeId, locationId, durationId);

            if (result.ReturnStatus == "success" && result.Data != null)
            {
                var dto = new MatchingCandidateViewDto
                {
                    pagination = _mapper.Map<PaginationDto>(result.Data.pagination),
                    candidates = result.Data.candidates != null
                        ? _mapper.Map<List<MatchingCandidateDto>>(result.Data.candidates)
                        : new List<MatchingCandidateDto>()
                };

                return new ApiResponse<MatchingCandidateViewDto>(
                    true,
                    dto,
                    "matching candidates fetched successfully",
                    ErrorCodes.Success
                );
            }

            return new ApiResponse<MatchingCandidateViewDto>(
                true,
                new MatchingCandidateViewDto(),
                "No matching candidate found",
                result.ErrorCode ?? ErrorCodes.Success
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetMatchingCandidatesAsync");
            throw;
        }
    }


}
