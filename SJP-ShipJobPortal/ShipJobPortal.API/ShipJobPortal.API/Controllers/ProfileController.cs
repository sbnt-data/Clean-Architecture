using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataHelpers;
using ShipJobPortal.Infrastructure.Repositories;

namespace ShipJobPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // This is important to access `User` from JWT
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ILogger<ProfileController> _logger;
    private readonly IDbExceptionLogger _dbExceptionLogger;

    public ProfileController(IProfileService profileService, ILogger<ProfileController> logger, IDbExceptionLogger dbExceptionLogger)
    {
        _profileService = profileService;
        _logger = logger;
        _dbExceptionLogger = dbExceptionLogger;
    }


    //[Authorize(Roles ="Seafarer")]
    [HttpPost("videoresume")]
    public async Task<IActionResult> UploadVideoResume([FromForm] VideoResumeFileDto file)
    {
        try
        {
            if (file == null || file.resumefile == null || file.resumefile.Length == 0)
            {
                return BadRequest(new ApiResponse<string>(false, null, "No video file uploaded", "ERR_NO_FILE"));
            }

            var result = await _profileService.UploadAndProcessResumeAsync(file);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UploadVideoResume");
            await _dbExceptionLogger.LogExceptionAsync("UploadVideoResume", ex.Message, ex.StackTrace);
            return StatusCode(500, new ApiResponse<string>(false, null, "Internal Server Error", "ERR500"));
        }
    }


    //[Authorize(Roles = "Seafarer")]
    [HttpPost("profileCreation")]//using 
    public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileDto userProfile)
    {
        if (userProfile == null)
            return BadRequest("User profile data is required.");

        try
        {
            var created = await _profileService.CreateUserProfileAsync(userProfile);
            return Ok(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CreateUserProfile");
            await _dbExceptionLogger.LogExceptionAsync("CreateUserProfile", ex.Message, ex.StackTrace);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("createNationality")]
    public async Task<IActionResult> NationalityCreate([FromBody] NationalityDto nationality)
    {
        if (nationality == null)
            return BadRequest("Nationality data is required.");

        try
        {
            var created = await _profileService.CreateNationalityAsync(nationality);
            return Ok(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in NationalityCreate");
            await _dbExceptionLogger.LogExceptionAsync("NationalityCreate", ex.Message, ex.StackTrace);
            return StatusCode(500, "Internal Server Error");
        }
    }

    //[Authorize(Roles = "Seafarer")]
    [HttpPost("applicantSaveFiles")]//using 
    public async Task<IActionResult> UserApplicantSaveFiles([FromBody] UserResumeandImageDto userProfile)
    {
        try
        {
            var created =  await _profileService.UserSaveFileAsync(userProfile);
            return Ok(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UserApplicantSaveFiles");
            await _dbExceptionLogger.LogExceptionAsync("applicantSaveFiles", ex.Message, ex.StackTrace);
            return StatusCode(500, "Internal Server Error");
        }
    }

    //[Authorize(Roles = "Seafarer")]
    [HttpGet("seafarer-profile")]
    public async Task<IActionResult> GetSeafarerProfile([FromQuery] int userId)
    {
        try
        {
            if (userId <= 0)
            {
                return BadRequest(new ApiResponse<string>(
                    false,
                    null,
                    "Invalid User ID",
                    ErrorCodes.BadRequest
                ));
            }

            var response = await _profileService.GetSeafarerProfileAsync(userId);

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            await _dbExceptionLogger.LogExceptionAsync("GetSeafarerProfile", ex.Message, ex.StackTrace);
            _logger.LogError(ex, "Exception occurred in GetSeafarerProfile");
            return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", "ERR500"));

        }

    }

    //[Authorize(Roles = "Seafarer")]
    [HttpPost("refer")]
    public async Task<IActionResult> ReferFriend([FromBody] ReferAFriendDto dto)
    {
        try
        {
            if (dto == null)
            {
                _logger.LogWarning("ReferFriend: Received null DTO");
                return BadRequest(new ApiResponse<string>(false, null, "Invalid input data", "ERR400"));
            }

            var result = await _profileService.ReferFriendAsync(dto);

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

    //[Authorize(Roles = "Recruiter")]
    [HttpGet("companyprofile")]
    public async Task<IActionResult> GetCompanyProfile([FromQuery] int companyId)
    {
        try
        {
            if (companyId <= 0)
            {
                return BadRequest(new ApiResponse<string>(
                    false,
                    null,
                    "Invalid User ID",
                    ErrorCodes.BadRequest
                ));
            }

            var response = await _profileService.GetcompanyProfileAsync(companyId);

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            await _dbExceptionLogger.LogExceptionAsync("GetSeafarerProfile", ex.Message, ex.StackTrace);
            _logger.LogError(ex, "Exception occurred in GetSeafarerProfile");
            return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", "ERR500"));

        }
    }

    //[Authorize(Roles ="Seafarer")]
    [HttpPatch("updateExperience")]
    public async Task<IActionResult> UpdateSeaExperience([FromForm] SeaExperianceViewPatchDto dto)
    {
        try
        {
            // Validate if the dto is null
            if (dto == null)
            {
                return BadRequest(new ApiResponse<string>(false, null, "Invalid user experience input", ErrorCodes.BadRequest));
            }

            // Call the service method to update the user experience
            var result = await _profileService.UpdateUserExperianceAsync(dto);

            // If the update was successful
            if (result.Success)
            {
                return Ok(result); // Returning success response with appropriate message
            }

            // If the update failed
            return BadRequest(result); // Returning error response with message
        }
        catch (Exception ex)
        {
            // Log the error and return server error response
            await _dbExceptionLogger.LogExceptionAsync("UpdateSeaExperience", ex.Message, ex.StackTrace);
            _logger.LogError(ex, "Error occurred in UpdateSeaExperience");
            return StatusCode(500, new ApiResponse<string>(false, null, "Internal Server Error", "ERR500"));
        }
    }

    //[Authorize(Roles ="Seafarer")]
    [HttpPatch("updateCertificates")]
    public async Task<IActionResult> UpdateCertificateDetails([FromForm] CertificatesViewPatchDto dto)
    {
        try
        {
            // Validate if the dto is null
            if (dto == null)
            {
                return BadRequest(new ApiResponse<string>(false, null, "Invalid user certificate input", ErrorCodes.BadRequest));
            }

            // Call the service method to update the user experience
            var result = await _profileService.UpdateUserDocumentCertificateAsync(dto);

            // If the update was successful
            if (result.Success)
            {
                return Ok(result); // Returning success response with appropriate message
            }

            // If the update failed
            return BadRequest(result); // Returning error response with message
        }
        catch (Exception ex)
        {
            // Log the error and return server error response
            await _dbExceptionLogger.LogExceptionAsync("UpdateCertificateDetails", ex.Message, ex.StackTrace);
            _logger.LogError(ex, "Error occurred in UpdateCertificateDetails");
            return StatusCode(500, new ApiResponse<string>(false, null, "Internal Server Error", "ERR500"));
        }
    }

    [HttpGet("videoresume/base64/{userId:int}")]
    public async Task<IActionResult> GetVideoResumeBase64(int userId)
    {
        try
        {
            var result = await _profileService.GetVideoResumeBase64Async(userId);

            if (!result.Success)
            {
                if (result.ErrorCode == "ERR404")
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            await _dbExceptionLogger.LogExceptionAsync("GetVideoResumeBase64", ex.Message, ex.StackTrace);
            _logger.LogError(ex, "Error in GetVideoResumeBase64 for userId: {UserId}", userId);
            return StatusCode(500, new ApiResponse<string>(false, null, "Internal Server Error", "ERR500"));
        }
    }


    [HttpGet("getUserfiles/{userId:int}")]
    public async Task<IActionResult> GetUserFiles(int userId)
    {
        try
        {
            var result = await _profileService.GetUserFilesAsync(userId);

            if (!result.Success)
            {
                if (result.ErrorCode == "ERR404")
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            await _dbExceptionLogger.LogExceptionAsync("GetVideoResumeBase64", ex.Message, ex.StackTrace);
            _logger.LogError(ex, "Error in GetVideoResumeBase64 for userId: {UserId}", userId);
            return StatusCode(500, new ApiResponse<string>(false, null, "Internal Server Error", "ERR500"));
        }
    }




}