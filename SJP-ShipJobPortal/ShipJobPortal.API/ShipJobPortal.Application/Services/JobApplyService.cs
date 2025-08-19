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

public class JobApplyService:IJobApplyService
{
    private readonly IJobApplyRepository _ApplyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<JobApplyService> _logger;

    public JobApplyService(IJobApplyRepository applyRepository, IMapper mapper, ILogger<JobApplyService> logger)
    {
        _ApplyRepository = applyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<AppliedCandidatesListDto>> GetAppliedCandidatesAsync(
int jobId,
int pageNumber = 1,
int pageSize = 10, int? positionId = 0,
int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, string? searchKey = "")
    {
        try
        {
            var result = await _ApplyRepository.GetAppliedCandidatesAsync(jobId, pageNumber, pageSize,positionId,vesselTypeId,locationId,durationId,searchKey);

            if (result.ReturnStatus == "success" && result.Data != null)
            {
                var dto = new AppliedCandidatesListDto
                {
                    pagination = _mapper.Map<PaginationDto>(result.Data.pagination),
                    CandidatesList = result.Data.CandidatesList != null
                        ? _mapper.Map<List<JobApplyDto>>(result.Data.CandidatesList)
                        : new List<JobApplyDto>()
                };

                return new ApiResponse<AppliedCandidatesListDto>(
                    true,
                    dto,
                    "Applied candidates fetched successfully",
                    ErrorCodes.Success
                );
            }

            return new ApiResponse<AppliedCandidatesListDto>(
                true,
                new AppliedCandidatesListDto(),
                "No candidates found",
                result.ErrorCode ?? ErrorCodes.Success
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAppliedCandidatesAsync");
            throw;
        }
    }

    public async Task<ApiResponse<string>> ApplyJobAsync(JobApplyPostDto model)
    {
        try
        {
            var mappedModel = _mapper.Map<JobApplyPostModel>(model);

            var result = await _ApplyRepository.ApplyJobAsync(mappedModel);

            if (result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                return new ApiResponse<string>
                (
                    true,
                    result.Data,
                    "Job application submitted successfully",
                    ErrorCodes.Success
                );
            }
            else if (result.ErrorCode == "ERR_DUPLICATE_APPLICATION")
            {
                return new ApiResponse<string>
                (
                    false,
                    null,
                    "You have already applied for this job.",
                    result.ErrorCode
                );
            }
            else
            {
                return new ApiResponse<string>
                (
                    false,
                    null,
                    "Failed to submit job application.",
                    result.ErrorCode
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in ApplyJobAsync service method");
            throw;
        }
    }

    public async Task<ApiResponse<AppliedJobsListDto>> GetAppliedJobsAsync(int UserId,int pageNumber = 1,int pageSize = 10,int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, string? searchKey = "")
    {
        try
        {
            var result = await _ApplyRepository.GetAppliedJobsAsync(UserId, pageNumber, pageSize,positionId,vesselTypeId,locationId,durationId,searchKey);

            if (result.ReturnStatus == "success" && result.Data != null)
            {
                var dto = new AppliedJobsListDto
                {
                    pagination = _mapper.Map<PaginationDto>(result.Data.pagination),
                    JobsList = result.Data.JobsList != null
                        ? _mapper.Map<List<AppliedJobDto>>(result.Data.JobsList)
                        : new List<AppliedJobDto>()
                };

                return new ApiResponse<AppliedJobsListDto>(
                    true,
                    dto,
                    "Applied jobs fetched successfully",
                    ErrorCodes.Success
                );
            }

            return new ApiResponse<AppliedJobsListDto>(
                true,
                new AppliedJobsListDto(),
                "No jobs found",
                result.ErrorCode ?? ErrorCodes.Success
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAppliedJobsAsync");
            throw;
        }
    }


    public async Task<ApiResponse<string>> SaveJobAsync(JobWishListDto model)
    {
        try
        {
            var mappedModel = _mapper.Map<JobWishlistModel>(model);

            var result = await _ApplyRepository.WishlistJobAsync(mappedModel);

            if (result.ReturnStatus == "Success" && result.ErrorCode == "ERR200")
            {
                return new ApiResponse<string>
                (
                    true,
                    result.Data,
                    model.Mode == "save"
                        ? "Job added to favorites successfully!"
                        : "Job removed from favorites successfully!",
                    ErrorCodes.Success
                );
            }
            else
            {
                return new ApiResponse<string>
                (
                    false,
                    null,
                    "Failed to save job application.",
                    result.ErrorCode
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in SaveJobAsync service method");
            throw;
        }
    }


    public async Task<ApiResponse<SavedJobsListDto>> GetSavedJobsAsync(int UserId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, int? monthValue = 0, string? searchKey = "")
    {
        try
        {
            var result = await _ApplyRepository.GetSavedJobsAsync(UserId, pageNumber, pageSize,positionId,vesselTypeId,locationId,durationId,monthValue, searchKey);

            if (result.ReturnStatus == "success" && result.Data != null)
            {
                var dto = new SavedJobsListDto
                {
                    pagination = _mapper.Map<PaginationDto>(result.Data.pagination),
                    JobsList = result.Data.JobsList != null
                        ? _mapper.Map<List<SavedJobsDto>>(result.Data.JobsList)
                        : new List<SavedJobsDto>()
                };

                return new ApiResponse<SavedJobsListDto>(
                    true,
                    dto,
                    "Saved jobs fetched successfully",
                    ErrorCodes.Success
                );
            }

            return new ApiResponse<SavedJobsListDto>(
                true,
                new SavedJobsListDto(),
                "No saved jobs found",
                result.ErrorCode ?? ErrorCodes.Success
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetSavedJobsAsync");
            throw;
        }
    }


    public async Task<ApiResponse<string>> UpdateJobApplicationStatusAsync(JobActiononCandidateDto model)
    {
        try
        {
            var mappedModel = _mapper.Map<JobActionOnCandidateModel>(model);

            var result = await _ApplyRepository.JobActionOnCandidateAsync(mappedModel);

            if (result.ReturnStatus == "success" && result.ErrorCode == "ERR200")
            {
                return new ApiResponse<string>
                (
                    true,
                    result.Data,
                    model.jobAction == "shortlisted"
                        ? "candidate shortlisted successfully!"
                        : "candidate rejected successfully!",
                    ErrorCodes.Success
                );
            }
            else
            {
                return new ApiResponse<string>
                (
                    false,
                    null,
                    "Failed to job action on application.",
                    result.ErrorCode
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateJobApplicationStatusAsync service method");
            throw;
        }
    }


}
