using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipJobPortal.API.Controllers;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IDbExceptionLogger _dbExceptionLogger;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger, IDbExceptionLogger dbExceptionLogger)
        {
            _roleService = roleService;
            _dbExceptionLogger = dbExceptionLogger;
            _logger = logger;
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Email)?.Value;
                //if (string.IsNullOrEmpty(username))
                //{
                //    return Unauthorized(new ApiResponse<string>(false, null, "User not authenticated"));
                //}

                var result = await _roleService.CreateRoleAsync(roleDto, username);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _dbExceptionLogger.LogExceptionAsync("CreateRole_Controller", ex.Message, ex.StackTrace);
                _logger.LogError(ex, "Error in CreateRole");
                return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", ErrorCodes.InternalServerError));
            }
        }


        [HttpGet("Get-AllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var result = await _roleService.GetAllRolesAsync();
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                await _dbExceptionLogger.LogExceptionAsync("GetAllRoles_Controller", ex.Message, ex.StackTrace);
                _logger.LogError(ex, "Error in GetAllRoles");
                return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", ErrorCodes.InternalServerError));
            }
        }

        [HttpPost("save-UserRoleMap")]
        public async Task<IActionResult> SaveUserRoleMapping([FromBody] UserRoleMapDto userRoleMapDto)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Email)?.Value;

                var response = await _roleService.SaveUserRoleMappingAsync(userRoleMapDto, username);

                return response.Success
                    ? Ok(response)
                    : BadRequest(response);
            }
            catch (Exception ex)
            {
                await _dbExceptionLogger.LogExceptionAsync("SaveUserRoleMapping_Controller", ex.Message, ex.StackTrace);
                _logger.LogError(ex, "Error in SaveUserRoleMapping");
                return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", ErrorCodes.InternalServerError));
            }
        }

        [HttpGet("get-UserRoleMap")]
        public async Task<IActionResult> GetUserRoleMapping()
        {
            try
            {
                var result = await _roleService.GetUserRoleMappingAsync();

                return result.Success
                    ? Ok(result)
                    : NotFound(result);
            }
            catch (Exception ex)
            {
                await _dbExceptionLogger.LogExceptionAsync("GetUserRoleMapping_Controller", ex.Message, ex.StackTrace);
                _logger.LogError(ex, "Error in GetUserRoleMapping");
                return StatusCode(500, new ApiResponse<string>(false, null, "Internal server error", ErrorCodes.InternalServerError));
            }
        }


    }
}
