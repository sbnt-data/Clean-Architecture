using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Domain.Entities;


namespace ShipJobPortal.Application.IServices;

public interface IJobPostService
{
    Task<ApiResponse<string>> CreateJobAsync(JobCreateDto dto);
    Task<ApiResponse<JobViewFilteredDto>> GetJobsFilteredAsync(int? vacancyId,int? userId,int? companyId, int CandidateId,int ?pageNumber, int ?pageSize,int? positionId = 0,int? vesselTypeId = 0,int? locationId = 0,int? durationId = 0,string? searchKey = "");
    Task<ApiResponse<string>> EditJobAsync(int vacancyId, Dictionary<string, object> patchData);
    Task<ApiResponse<string>> DeleteJobAsync(int vacancyId);

    Task<ApiResponse<string>> JobViewCount(JobViewCountDto dto); 

    //Task<ApiResponse<IEnumerable<jobViewDto>>> GetAllJobsAsync(int? vacancyId = null);
    //Task<ApiResponse<IEnumerable<jobViewDto>>> GetAllJobyUserAsync(int? userId, int? companyId);
    //Task<IEnumerable<JobCreateDto>> GetAllJobsAsync(int? vacancyId = null);
    //Task<ReturnResult> DeleteJobAsync(int vacancyId);
}
