using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Controllers;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchingController : ControllerBase
    {
        private readonly IMatchingService _matchingServices;
        private readonly ILogger<MatchingController> _logger;
        private readonly IDbExceptionLogger _dbExceptionLogger;
        private readonly IConfiguration _configuration;

        public MatchingController(IMatchingService matchingServices, ILogger<MatchingController> logger, IDbExceptionLogger dbExceptionLogger, IConfiguration configuration)
        {
            _matchingServices = matchingServices;
            _logger = logger;
            _dbExceptionLogger = dbExceptionLogger;
            _configuration = configuration;
        }

        //[Authorize(Roles ="Recruiter")]
        [HttpPost("sendCandidateDetails")]
        public async Task<IActionResult> SendCandidatetoShipHire([FromBody] CandidateListtoShiphireDto model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new ApiResponse<string>(
                        false,
                        null,
                        "Invalid User ID",
                        ErrorCodes.BadRequest
                    ));
                }

                var response = await _matchingServices.SendCandidatestoShipHire(model);

                if (!response.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendCandidatetoShipHire");
                await _dbExceptionLogger.LogExceptionAsync("SendCandidatetoShipHire", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }

        }


        //[Authorize(Roles = "Recruiter")]
        [HttpGet("matchingCandidate")]
        public async Task<IActionResult> GetMatchingCandidateForJob([FromQuery] int jobId,[FromQuery] int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
[FromQuery] int? positionId = 0, [FromQuery] int? vesselTypeId = 0, [FromQuery] int? locationId = 0, [FromQuery] int? durationId = 0)
        {
            try
            {
                //if (jobId <= 0)
                //{
                //    return BadRequest(new ApiResponse<string>(
                //        false,
                //        null,
                //        "Invalid job ID",
                //        ErrorCodes.BadRequest
                //    ));
                //}

                var response = await _matchingServices.GetMatchingCandidatesAsync(jobId,userId, pageNumber, pageSize, positionId, vesselTypeId, locationId, durationId);

                if (!response.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMatchingCandidateForJob");
                await _dbExceptionLogger.LogExceptionAsync("GetMatchingCandidateForJob", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }
        }


    }
}
