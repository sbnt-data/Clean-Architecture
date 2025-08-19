using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Application.Services;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.WebAPI.Controllers;

namespace ShipJobPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobApplyController : ControllerBase
    {
        private readonly IJobApplyService _ApplyService;
        private readonly ILogger<JobApplyController> _logger;
        private readonly IDbExceptionLogger _dbExceptionLogger;

        public JobApplyController(IJobApplyService jobApplyService, ILogger<JobApplyController> logger, IDbExceptionLogger dbExceptionLogger)
        {
            _ApplyService = jobApplyService;
            _logger = logger;
            _dbExceptionLogger = dbExceptionLogger;
        }

        //[Authorize(Roles = "Recruiter")]
        [HttpGet("applied-candidates")]
        public async Task<IActionResult> GetAppliedCandidates(
    [FromQuery] int jobId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10, [FromQuery] int? positionId = 0,
[FromQuery] int? vesselTypeId = 0, [FromQuery] int? locationId = 0,
[FromQuery] int? durationId = 0, [FromQuery] string? searchKey = "")
        {
            try
            {
                if (jobId <= 0)
                {
                    return BadRequest(new ApiResponse<string>(
                        false,
                        null,
                        "Invalid Job ID",
                        ErrorCodes.BadRequest
                    ));
                }

                var response = await _ApplyService.GetAppliedCandidatesAsync(jobId, pageNumber, pageSize, positionId, vesselTypeId, locationId, durationId, searchKey);

                if (!response.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAppliedCandidates");
                await _dbExceptionLogger.LogExceptionAsync("GetAppliedCandidates", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }
        }

        //[Authorize(Roles = "Seafarer")]
        [HttpPost("apply-job")]
        public async Task<IActionResult> ApplyJob([FromBody] JobApplyPostDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(
                    false,
                    null,
                    "Invalid request data.",
                    ErrorCodes.BadRequest
                ));
            }

            try
            {
                var response = await _ApplyService.ApplyJobAsync(model);

                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ApplyJob endpoint.");
                await _dbExceptionLogger.LogExceptionAsync("ApplyJob", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }

        //[Authorize(Roles = "Seafarer")]
        [HttpGet("applied-Jobs")]
        public async Task<IActionResult> GetAppliedJobsForCandidate(
[FromQuery] int UserId,
[FromQuery] int pageNumber = 1,
[FromQuery] int pageSize = 10, [FromQuery] int? positionId = 0,
[FromQuery] int? vesselTypeId = 0, [FromQuery] int? locationId = 0,
[FromQuery] int? durationId = 0, [FromQuery] string? searchKey = "")
        {


            try
            {
                if (UserId <= 0)
                {
                    return BadRequest(new ApiResponse<string>(
                        false,
                        null,
                        "Invalid Job ID",
                        ErrorCodes.BadRequest
                    ));
                }

                var response = await _ApplyService.GetAppliedJobsAsync(UserId, pageNumber, pageSize, positionId, vesselTypeId, locationId, durationId, searchKey);

                if (!response.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAppliedJobsForCandidate");
                await _dbExceptionLogger.LogExceptionAsync("GetAppliedJobsForCandidate", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }
        }

        //[Authorize(Roles = "Seafarer")]
        [HttpPost("favouriteJob")]
        public async Task<IActionResult> SaveJobtowishlist([FromBody] JobWishListDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(
                    false,
                    null,
                    "Invalid request data.",
                    ErrorCodes.BadRequest
                ));
            }

            try
            {
                var response = await _ApplyService.SaveJobAsync(model);

                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in SaveJobtowishlist endpoint.");
                await _dbExceptionLogger.LogExceptionAsync("SaveJobtowishlist", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }

        //[Authorize(Roles = "Seafarer")]
        [HttpGet("savedJobs")]
        public async Task<IActionResult> GetSavedJobsForCandidate([FromQuery] int UserId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
[FromQuery] int? positionId = 0,
[FromQuery] int? vesselTypeId = 0, [FromQuery] int? locationId = 0,
[FromQuery] int? durationId = 0,[FromQuery]int?monthValue=0, [FromQuery] string? searchKey = "")
        {
            try
            {
                if (UserId <= 0)
                {
                    return BadRequest(new ApiResponse<string>(
                        false,
                        null,
                        "Invalid User ID",
                        ErrorCodes.BadRequest
                    ));
                }

                var response = await _ApplyService.GetSavedJobsAsync(UserId, pageNumber, pageSize, positionId, vesselTypeId, locationId, durationId,monthValue, searchKey);

                if (!response.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSavedJobsForCandidate");
                await _dbExceptionLogger.LogExceptionAsync("GetSavedJobsForCandidate", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }

        }

        //[Authorize(Roles = "Seafarer")]
        [HttpPost("jobAction")]
        public async Task<IActionResult> JobActiononCandidateAsync([FromBody] JobActiononCandidateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(
                    false,
                    null,
                    "Invalid request data.",
                    ErrorCodes.BadRequest
                ));
            }

            try
            {
                var response = await _ApplyService.UpdateJobApplicationStatusAsync(model);

                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in JobActiononCandidateAsync endpoint.");
                await _dbExceptionLogger.LogExceptionAsync("JobActiononCandidateAsync", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }

    }
}
