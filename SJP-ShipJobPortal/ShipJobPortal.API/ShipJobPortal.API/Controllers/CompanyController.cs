using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Interfaces;
using Azure;
using ShipJobPortal.Domain.Constants;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace ShipJobPortal.WebAPI.Controllers
{
    /// <summary>
    /// This controller work fine with V1 dapper
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // This is important to access `User` from JWT
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;
        private readonly IDbExceptionLogger _dbExceptionLogger;

        public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger, IDbExceptionLogger dbExceptionLogger)
        {
            _companyService = companyService;
            _logger = logger;
            _dbExceptionLogger = dbExceptionLogger;
        }

        [HttpPost("createCompany")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDto companyCreateDto)
        {

            var username = User.FindFirst(ClaimTypes.Email)?.Value;
            //if (string.IsNullOrEmpty(username))
            //    return Unauthorized(new { Message = "User not authenticated" });

            try
            {
                var result = await _companyService.CreateCompanyAsync(companyCreateDto, username);

                return result.Success
        ? Ok(result)
        : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating company");
                await _dbExceptionLogger.LogExceptionAsync("CreateCompany_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "An unexpected error occurred.", ErrorCodes.InternalServerError));
            }
        }

        [HttpGet("getAllCompanies")]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var response = await _companyService.GetAllCompaniesAsync();

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<CompanyDropDto>>(
                        false,
                        null,
                        "Failed to retrieve companies.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<CompanyDropDto>>(
                        false,
                        null,
                        "No companies found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<CompanyDropDto>>(
                    true,
                    response.Data,
                    "Companies retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving companies.");
                await _dbExceptionLogger.LogExceptionAsync("GetAllCompanies_Controller", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }


        [HttpGet("getCompany")]
        public async Task<IActionResult> GetCompany(int CompanyId)
        {
            try
            {
                var response = await _companyService.GetCompanyAsync(CompanyId);

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<CompanyDto>>(
                        false,
                        null,
                        "Failed to retrieve companies.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<CompanyDto>>(
                        false,
                        null,
                        "No companies found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<CompanyDto>>(
                    true,
                    response.Data,
                    "Companies retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving companies.");
                await _dbExceptionLogger.LogExceptionAsync("GetAllCompanies_Controller", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }
    }
}
