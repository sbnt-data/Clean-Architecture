using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Application.Services;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // This is important to access `User` from JWT
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly ILogger<ModuleController> _logger;
        private readonly IDbExceptionLogger _dbExceptionLogger;

        public ModuleController(IModuleService moduleService, ILogger<ModuleController> logger, IDbExceptionLogger dbExceptionLogger)
        {
            _moduleService = moduleService;
            _logger = logger;
            _dbExceptionLogger = dbExceptionLogger;
        }

        [HttpPost("createModule")]
        public async Task<IActionResult> CreateModule([FromBody] ModuleDto dto)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Email)?.Value;
                var result = await _moduleService.CreateModuleAsync(dto, username);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in CreateModule controller");
                await _dbExceptionLogger.LogExceptionAsync("CreateModule_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", ErrorCodes.InternalServerError));
            }
        }

        [HttpGet("Get-Modules/{mode}")]
        public async Task<IActionResult> GetAllMainModules(string mode)
        {
            try
            {
                var result = await _moduleService.GetAllMainModulesAsync(mode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllMainModules");
                await _dbExceptionLogger.LogExceptionAsync("GetAllMainModules_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", ErrorCodes.InternalServerError));
            }
        }

        [HttpGet("Get-RoleModuleMap")]
        public async Task<IActionResult> GetRoleModuleMap()
        {
            try
            {
                //var username = User.FindFirst(ClaimTypes.Email)?.Value;
                var result = await _moduleService.GetRoleModuleMapAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRoleModuleMap controller");
                await _dbExceptionLogger.LogExceptionAsync("GetRoleModuleMap_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<string>(false, null, "Internal Server Error", ErrorCodes.InternalServerError));
            }
        }

        [HttpPost("Save-RolemoduleMapping")]
        public async Task<IActionResult> SaveRoleModuleMap([FromBody] RoleModuleMapDto dto)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Email)?.Value;

                var response = await _moduleService.SaveRoleModuleMapAsync(dto, username);

                return response.Success
                    ? Ok(response)
                    : BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveRoleModuleMap controller");
                await _dbExceptionLogger.LogExceptionAsync("SaveRoleModuleMap_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<object>(false, null, "Internal Server Error", ErrorCodes.InternalServerError));
            }
        }


    }

}