using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Application.Services;

public class JobPostService : IJobPostService
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<JobPostService> _logger;

    public JobPostService(IJobPostRepository jobRepository, IMapper mapper, ILogger<JobPostService> logger)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<string>> CreateJobAsync(JobCreateDto dto)
    {
        try
        {
            if (dto == null)
                return new ApiResponse<string>(false, null, "Invalid job input", ErrorCodes.BadRequest);

            var job = _mapper.Map<JobPostModel>(dto);
            job.Status = "Active";

            var result = await _jobRepository.CreateorUpdateJobAsync(job);

            if (result.ReturnStatus == "inserted")
            {
                return new ApiResponse<string>(true, null, "Job created/updated successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, "Failed to save job", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in CreateJobAsync");
            throw;
        }
    }

    public async Task<ApiResponse<JobViewFilteredDto>> GetJobsFilteredAsync(
int? vacancyId,
int? userId,
int? companyId,
int CandidateId,
int ?pageNumber=1,int ?pageSize=7,
int? positionId = 0,
int? vesselTypeId = 0,
int? locationId = 0,
int? durationId = 0,
string? searchKey = "")
    {
        try
        {
            var result = await _jobRepository.GetAllJobsFilteredAsync(
                vacancyId, userId, companyId,CandidateId, pageNumber,pageSize,
                positionId, vesselTypeId, locationId, durationId, searchKey
            );

            if (result.ReturnStatus == "success" && result.Data != null)
            {
                var filteredDto = new JobViewFilteredDto
                {
                    Pagination = _mapper.Map<PaginationDto>(result.Data.Pagination),
                    JobsList = result.Data.JobsList != null
                        ? _mapper.Map<List<jobViewDto>>(result.Data.JobsList)
                        : new List<jobViewDto>()
                };

                return new ApiResponse<JobViewFilteredDto>(
                    true,
                    filteredDto,
                    "Jobs fetched successfully",
                    ErrorCodes.Success
                );
            }

            return new ApiResponse<JobViewFilteredDto>(
                true,
                new JobViewFilteredDto(),
                "No jobs found",
                result.ErrorCode ?? ErrorCodes.Success
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetJobsFilteredAsync");
            throw;
        }
    }

    //public async Task<ApiResponse<JobViewFilteredDto>> GetJobsFilteredAsync(int? vacancyId,
    //    int? userId, int? companyId, int pageNumber,
    //    int? positionId = 0, int? vesselTypeId = 0,
    //    int? locationId = 0, int? durationId = 0,
    //    string? searchKey = "")
    //{
    //    try
    //    {
    //        var result = await _jobRepository.GetAllJobsFilteredAsync(vacancyId,
    //            userId, companyId, pageNumber,
    //            positionId, vesselTypeId, locationId, durationId, searchKey);

    //        if (result.ReturnStatus == "success" && result.Data != null)
    //        {
    //            // result.Data is already a JobViewModel_test (strongly typed)
    //            var jobPaginationModel = result.Data;

    //            // ✅ Map pagination and metadata
    //            var paginationDto = _mapper.Map<JobViewFilteredDto>(jobPaginationModel);

    //            // ✅ Map Job list
    //            if (jobPaginationModel.JobsList != null && jobPaginationModel.JobsList.Any())
    //            {
    //                paginationDto.JobsList = _mapper.Map<List<jobViewDto>>(jobPaginationModel.JobsList);
    //            }

    //            return new ApiResponse<JobViewFilteredDto>(
    //                true,
    //                paginationDto,
    //                "Jobs fetched successfully",
    //                ErrorCodes.Success
    //            );
    //        }

    //        return new ApiResponse<JobViewFilteredDto>(
    //            false,
    //            null,
    //            "No jobs found",
    //            result.ErrorCode ?? ErrorCodes.NotFound
    //        );
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error occurred in GetJobsFilteredAsync");
    //        return new ApiResponse<JobViewFilteredDto>(
    //            false,
    //            null,
    //            "Internal server error",
    //            ErrorCodes.InternalServerError
    //        );
    //    }
    //}

    public async Task<ApiResponse<string>> EditJobAsync(int vacancyId, Dictionary<string, object> patchData)
    {
        try
        {
            if (vacancyId <= 0 || patchData == null || patchData.Count == 0)
                return new ApiResponse<string>(false, null, "Invalid job input", ErrorCodes.BadRequest);

            // Convert patch dictionary to JobPatchDto
            var json = JsonConvert.SerializeObject(patchData);
            var patchDto = JsonConvert.DeserializeObject<JobPatchDto>(json);

            if (patchDto == null)
                return new ApiResponse<string>(false, null, "Invalid patch data", ErrorCodes.BadRequest);

            // Map only the patch data to a new JobPostModel
            var jobToUpdate = new JobPostModel
            {
                JobId = vacancyId,
                Status = "Active" // or keep null if the SP sets default
            };

            _mapper.Map(patchDto, jobToUpdate);

            // Fix SQL datetime overflow for OpenDate and CloseDate
            if (jobToUpdate.OpenDate < new DateTime(1753, 1, 1))
                jobToUpdate.OpenDate = null;
            if (jobToUpdate.CloseDate < new DateTime(1753, 1, 1))
                jobToUpdate.CloseDate = null;

            // Save using existing SP
            var saveResult = await _jobRepository.CreateorUpdateJobAsync(jobToUpdate);

            return saveResult.ReturnStatus?.ToLower() == "updated"
                ? new ApiResponse<string>(true, null, "Job updated successfully", ErrorCodes.Success)
                : new ApiResponse<string>(false, null, "Failed to update job", saveResult.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in EditJobAsync");
            throw;
        }
    }

    public async Task<ApiResponse<string>> DeleteJobAsync(int vacancyId)
    {
        try
        {
            var result = await _jobRepository.DeleteJobAsync(vacancyId);

            if (result.ReturnStatus == "Deleted")
            {
                return new ApiResponse<string>(true, null, "Job deleted successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, "Failed to delete job", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in DeleteJobAsync");
            throw;
        }
    }


    public async Task<ApiResponse<string>> JobViewCount(JobViewCountDto dto)
    {
        try
        {

            var model = _mapper.Map<JobViewCountModel>(dto);
            var result = await _jobRepository.JobViewCountAsync(model);

            if (result.ReturnStatus == "success")
            {
                //here email becuase db insertion is success
                return new ApiResponse<string>(true, null, "view successful", result.ErrorCode);
            }

            if (result.ReturnStatus == "already seen")
                return new ApiResponse<string>(false, null, result.ReturnStatus, result.ErrorCode);

            return new ApiResponse<string>(false, null, "failed", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in JobViewCount");
            throw;
        }
    }




    /*Unused
    public async Task<ApiResponse<IEnumerable<jobViewDto>>> GetAllJobsAsync(int? vacancyId = null)
    {
        try
        {
            var result = await _jobRepository.GetAllJobsAsync(vacancyId);

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var jobDtos = _mapper.Map<IEnumerable<jobViewDto>>(result.Data);
                return new ApiResponse<IEnumerable<jobViewDto>>(true, jobDtos, "Jobs fetched successfully", ErrorCodes.Success);
            }

            return new ApiResponse<IEnumerable<jobViewDto>>(false, null, "No jobs found", result.ErrorCode ?? ErrorCodes.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAllJobsAsync");
            return new ApiResponse<IEnumerable<jobViewDto>>(false, null, "Internal server error", ErrorCodes.InternalServerError);
        }
    }
    public async Task<ApiResponse<IEnumerable<jobViewDto>>> GetAllJobyUserAsync(int? userId, int? companyId)
    {
        try
        {
            var result = await _jobRepository.GetAllJobsbyUserAsync(userId, companyId);

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var jobDtos = _mapper.Map<IEnumerable<jobViewDto>>(result.Data);
                return new ApiResponse<IEnumerable<jobViewDto>>(true, jobDtos, "Jobs fetched successfully", ErrorCodes.Success);
            }

            return new ApiResponse<IEnumerable<jobViewDto>>(false, null, "No jobs found", result.ErrorCode ?? ErrorCodes.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAllJobsAsync");
            return new ApiResponse<IEnumerable<jobViewDto>>(false, null, "Internal server error", ErrorCodes.InternalServerError);
        }
    }




    //public async Task<ApiResponse<string>> EditJobAsync(int vacancyId, Dictionary<string, object> patchData)
    //{
    //    try
    //    {
    //        if (vacancyId <= 0 || patchData == null || patchData.Count == 0)
    //            return new ApiResponse<string>(false, null, "Invalid job input", ErrorCodes.BadRequest);

    //        // Get wrapped result from repository
    //        var resultWrapper = await _jobRepository.GetJobByIdAsync(vacancyId);

    //        // Extract single job from result
    //        var existingJob = resultWrapper?.Data?.FirstOrDefault();

    //        if (existingJob == null)
    //            return new ApiResponse<string>(false, null, "Job not found", ErrorCodes.NotFound);

    //        // Convert patch dictionary to JobPatchDto
    //        var json = JsonConvert.SerializeObject(patchData);
    //        var patchDto = JsonConvert.DeserializeObject<JobPatchDto>(json);

    //        if (patchDto == null)
    //            return new ApiResponse<string>(false, null, "Invalid patch data", ErrorCodes.BadRequest);

    //        // Apply patch
    //        _mapper.Map(patchDto, existingJob);
    //        existingJob.VacancyId = vacancyId;
    //        existingJob.Status = "Active";

    //        // Save using existing CreateJobAsync
    //        var saveResult = await _jobRepository.CreateorUpdateJobAsync(existingJob);

    //        return saveResult.ReturnStatus?.ToLower() == "updated"
    //            ? new ApiResponse<string>(true, null, "Job updated successfully", ErrorCodes.Success)
    //            : new ApiResponse<string>(false, null, "Failed to update job", saveResult.ErrorCode);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error occurred in EditJobAsync");
    //        return new ApiResponse<string>(false, null, "Internal server error", ErrorCodes.InternalServerError);
    //    }
    //}

    //*/
}


/*
using AutoMapper;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Application.Services
{
public class JobPostService : IJobPostService
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;
    public readonly ILogger<JobPostService> _logger;

    public JobPostService(IJobPostRepository jobRepository, IMapper mapper, ILogger<JobPostService> logger)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<object> JobCreation(JobCreateDto dto)
    {
        try
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var job = _mapper.Map<JobPostModel>(dto);
            job.Status = "Active";

            return await _jobRepository.CreateJobAsync(job);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in JobCreation");
            throw;
            //return new ReturnResult { ReturnStatus = "error", ErrorCode = "ERR500" };
        }
    }

    public async Task<IEnumerable<JobCreateDto>> GetAllJobsAsync(int? vacancyId = null)
    {
        try
        {
            var jobs = await _jobRepository.GetAllJobsAsync(vacancyId);
            return _mapper.Map<IEnumerable<JobCreateDto>>(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAllJobsAsync");
            throw;
            //return Enumerable.Empty<JobCreateDto>();
        }
    }

    public async Task<ReturnResult> DeleteJobAsync(int vacancyId)
    {
        try
        {
            return await _jobRepository.DeleteJobAsync(vacancyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in DeleteJobAsync");
            throw;
            //return new ReturnResult { ReturnStatus = "error", ErrorCode = "ERR500" };
        }
    }
}
}


*/