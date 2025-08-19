using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShipJobPortal.Application.DTOs;

namespace ShipJobPortal.Application.IServices;

public interface IJobApplyService
{
    Task<ApiResponse<AppliedCandidatesListDto>> GetAppliedCandidatesAsync(int jobId,int pageNumber = 1,int pageSize = 10, int? positionId = 0,
int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, string? searchKey = "");
    Task<ApiResponse<string>> ApplyJobAsync(JobApplyPostDto model);
    Task<ApiResponse<AppliedJobsListDto>> GetAppliedJobsAsync(int UserId,int pageNumber = 1,int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0,int? durationId = 0,  string? searchKey = "");

    Task<ApiResponse<string>> SaveJobAsync(JobWishListDto model);

    Task<ApiResponse<SavedJobsListDto>> GetSavedJobsAsync(int UserId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0,int?monthValue=0, string? searchKey = "");
    Task<ApiResponse<string>> UpdateJobApplicationStatusAsync(JobActiononCandidateDto model);
}
