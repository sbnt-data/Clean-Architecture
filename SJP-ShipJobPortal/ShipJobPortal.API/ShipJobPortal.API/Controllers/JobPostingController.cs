using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Application.Services;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Interfaces;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace ShipJobPortal.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostController : ControllerBase
    {
        private readonly IJobPostService _jobService;
        private readonly ILogger<JobPostController> _logger;
        private readonly IDbExceptionLogger _dbExceptionLogger;

        public JobPostController(IJobPostService jobService, ILogger<JobPostController> logger, IDbExceptionLogger dbExceptionLogger)
        {
            _jobService = jobService;
            _logger = logger;
            _dbExceptionLogger = dbExceptionLogger;
        }

        //[Authorize(Roles = "Recruiter")]
        [HttpPost("createJobVacancy")]
        public async Task<IActionResult> CreateJob([FromBody] JobCreateDto dto)
        {
            //var username = User.FindFirst(ClaimTypes.Email)?.Value ?? "system";

            try
            {
                var result = await _jobService.CreateJobAsync(dto);

                return result.Success
                    ? Ok(result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in CreateJob");
                await _dbExceptionLogger.LogExceptionAsync("CreateJob_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Unexpected error", ErrorCodes.InternalServerError));
            }
        }

        //[Authorize(Roles = "Seafarer")]
        [HttpGet("getJobVacancy")]
        public async Task<IActionResult> GetAllJobs([FromQuery] int ?pageNumber=1,[FromQuery]int ?pageSize=8, [FromQuery] int? vacancyId = 0, [FromQuery] int? positionId = 0, [FromQuery] int? vesselTypeId = 0, [FromQuery] int? locationId = 0, [FromQuery] int? durationId = 0, [FromQuery] string? searchKey = "", [FromQuery] int CandidateId = 0)
        {
            try
            {
                var result = await _jobService.GetJobsFilteredAsync(
                    vacancyId, 0, 0,CandidateId, pageNumber,pageSize,
                    positionId, vesselTypeId, locationId, durationId, searchKey);

                if (!result.Success)
                    return StatusCode(500, result);

                if (result.Data?.JobsList == null || !result.Data.JobsList.Any())
                    return Ok(new ApiResponse<JobViewFilteredDto>(true,new JobViewFilteredDto(), "No jobs found", ErrorCodes.Success));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllJobs");
                await _dbExceptionLogger.LogExceptionAsync("GetAllJobs_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Unexpected error", ErrorCodes.InternalServerError));
            }
        }

        //[Authorize(Roles = "Recruiter")]
        [HttpGet("getJobsbyUser")]
        public async Task<IActionResult> GetAllJobsbyUser([FromQuery] int? userId, [FromQuery] int? companyId, [FromQuery] int ?pageNumber=1,[FromQuery]int ?pageSize=8, [FromQuery] int? positionId = 0, [FromQuery] int? vesselTypeId = 0, [FromQuery] int? locationId = 0, [FromQuery] int? durationId = 0, [FromQuery] string? searchKey = "")
        {
            try
            {
                var result = await _jobService.GetJobsFilteredAsync(
                    -1, userId, companyId,0, pageNumber,pageSize,
                    positionId, vesselTypeId, locationId, durationId, searchKey);

                if (!result.Success)
                    return StatusCode(500, result);

                if (result.Data?.JobsList == null || !result.Data.JobsList.Any())
                    return Ok(new ApiResponse<JobViewFilteredDto>(true, new JobViewFilteredDto(), "No jobs found", ErrorCodes.Success));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllJobsByUser");
                await _dbExceptionLogger.LogExceptionAsync("GetAllJobsByUser_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Unexpected error", ErrorCodes.InternalServerError));
            }
        }

        //[Authorize(Roles = "Recruiter")]
        [HttpPatch("editJobVacancy")]
        public async Task<IActionResult> EditJob(int jobId, [FromBody] JobPatchDto patchDto)
        {
            var json = JsonConvert.SerializeObject(patchDto);
            var patchData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);


            try
            {
                JobCreateDto dto = new();
                var result = await _jobService.EditJobAsync(jobId, patchData);

                return result.Success
                    ? Ok(result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in EditJob");
                await _dbExceptionLogger.LogExceptionAsync("EditJob", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Unexpected error", ErrorCodes.InternalServerError));
            }
        }

        //[Authorize(Roles = "Recruiter")]
        [HttpPost("delete/{jobId}")]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            try
            {
                var result = await _jobService.DeleteJobAsync(jobId);

                return result.Success
                    ? Ok(result)
                    : StatusCode(500, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in DeleteJob");
                await _dbExceptionLogger.LogExceptionAsync("DeleteJob_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Unexpected error", ErrorCodes.InternalServerError));
            }
        }

        //[Authorize(Roles = "Seafarer")]
        [HttpPost("jobcount")]
        public async Task<IActionResult> jobcount([FromBody] JobViewCountDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("ReferFriend: Received null DTO");
                    return BadRequest(new ApiResponse<string>(false, null, "Invalid input data", "ERR400"));
                }

                var result = await _jobService.JobViewCount(dto);

                if (result.Success)
                    return Ok(result);

                _logger.LogWarning("Referral failed: {message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                await _dbExceptionLogger.LogExceptionAsync("ReferFriend", ex.Message, ex.StackTrace);
                _logger.LogError(ex, "Exception occurred in ReferFriend");
                return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", "ERR500"));
            }
        }

        //[HttpGet("getJobsbyId")]
        //public async Task<IActionResult> GetJobsbyId([FromQuery] int? userId, [FromQuery] int? companyId, [FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 8, [FromQuery] int? positionId = 0, [FromQuery] int? vesselTypeId = 0, [FromQuery] int? locationId = 0, [FromQuery] int? durationId = 0, [FromQuery] string? searchKey = "")
        //{
        //    try
        //    {
        //        var result = await _jobService.GetJobsFilteredAsync(
        //            -1, userId, companyId, 0, pageNumber, pageSize,
        //            positionId, vesselTypeId, locationId, durationId, searchKey);

        //        if (!result.Success)
        //            return StatusCode(500, result);

        //        if (result.Data?.JobsList == null || !result.Data.JobsList.Any())
        //            return Ok(new ApiResponse<JobViewFilteredDto>(true, new JobViewFilteredDto(), "No jobs found", ErrorCodes.Success));

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Exception occurred in GetAllJobsByUser");
        //        await _dbExceptionLogger.LogExceptionAsync("GetAllJobsByUser_Controller", ex.Message, ex.StackTrace);
        //        return StatusCode(500, new ApiResponse<string>(false, null, "Unexpected error", ErrorCodes.InternalServerError));
        //    }
        //}
    }
}


/*



        [HttpGet("getJobsbyUser_test")]
        public async Task<IActionResult> GetAllJobsbyUser_test([FromQuery] int? userId,[FromQuery] int? companyId,[FromQuery] int pageNumber,[FromQuery] int? positionId = 0,[FromQuery] int? vesselTypeId = 0,[FromQuery] int? locationId = 0,[FromQuery] int? durationId = 0,[FromQuery] string? searchKey = "")
        {
            try
            {
                var result = await _jobService.GetJobsFilteredAsync(-1,
                    userId, companyId, pageNumber, positionId, vesselTypeId, locationId, durationId, searchKey);

                if (!result.Success)
                    return StatusCode(500, result);

                if (result.Data == null || result.Data.JobsList == null || !result.Data.JobsList.Any())
                    return NotFound(new ApiResponse<JobViewFilteredDto>(false, null, "No jobs found", ErrorCodes.NotFound));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllJobsbyUser_test");
                await _dbExceptionLogger.LogExceptionAsync("GetAllJobsbyUser_test_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Unexpected error", ErrorCodes.InternalServerError));
            }
        }






using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // This is important to access `User` from JWT
    public class JobPostingController : ControllerBase
    {
        private readonly IJobPostService _jobService;
        private readonly ILogger<JobPostingController> _logger;
        private readonly IDbExceptionLogger _dbExceptionLogger;
        public JobPostingController(IJobPostService jobService, ILogger<JobPostingController> logger, IDbExceptionLogger dbExceptionLogger)
        {
            _jobService = jobService;
            _logger = logger;
            _dbExceptionLogger = dbExceptionLogger;
        }

        [HttpPost("createJobpost")]
        public async Task<IActionResult> JobCreation([FromBody] JobCreateDto JobCreateDto)
        {
            if (JobCreateDto == null)
                return BadRequest("Job details are required.");
            try
            {
                var result = await _jobService.CreateJobAsync(JobCreateDto);
                if (result != null)
                    return Ok(result);

                return BadRequest("Job creation failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in JobCreation endpoint.");
                await _dbExceptionLogger.LogExceptionAsync("JobCreation_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal server error.");
            }

        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            try
            {
                var result = await _jobService.DeleteJobAsync(id);

                if (result != null && result.ReturnStatus=="success")
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in DeleteJob endpoint for id {id}.");
                await _dbExceptionLogger.LogExceptionAsync("DeleteJob_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal server error.");
            }

        }

        [HttpGet("all")]
        public async Task<IActionResult> GetJob([FromQuery] int? id)
        {
            try
            {
                var result = await _jobService.GetAllJobsAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetJob endpoint.");
                await _dbExceptionLogger.LogExceptionAsync("GetJob_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal server error.");
            }

        }
    }
}
*/