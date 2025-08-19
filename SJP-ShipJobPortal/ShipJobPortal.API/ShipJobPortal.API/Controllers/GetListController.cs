using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetListController : ControllerBase
    {
        /// <summary>
        /// Working status : fine 
        /// List : 
        /// GetAllCountry
        /// getAllState
        /// GetAllCity
        /// </summary>
        private readonly IGetListService _listService;
        private readonly ILogger<GetListController> _logger;
        private readonly IDbExceptionLogger _dbExceptionLogger;
        public GetListController(IGetListService listService, ILogger<GetListController> logger, IDbExceptionLogger dbExceptionLogger)
        {
            _listService = listService;
            _logger = logger;
            _dbExceptionLogger = dbExceptionLogger;
        }

        /// <summary> 18-july 2025
        /// to Get countries for drop dow menu
        /// </summary>
        /// <returns>list of countries via GetCountryListDto</returns>
        [HttpGet("getAllCountries")]
        public async Task<IActionResult> GetAllCountry()
        {
            try
            {
                var response = await _listService.GetAllCountryAsync();

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<GetCountryListDto>>(
                        false,
                        null,
                        "Failed to retrieve countries.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<GetCountryListDto>>(
                        false,
                        null,
                        "No countries found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<GetCountryListDto>>(
                    true,
                    response.Data,
                    "countries retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving countries.");
                await _dbExceptionLogger.LogExceptionAsync("GetAllCountry_Controller", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }

        /// <summary> 18 july 2025
        /// to Get state for drop dow menu
        /// </summary>
        /// <returns>list of countries via GetStateListDto</returns>
        [HttpGet("getAllstates")]
        public async Task<IActionResult> getAllState(int?countryId)
        {
            try
            {
                var response = await _listService.GetAllStateAsync(countryId);

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<GetStateListDto>>(
                        false,
                        null,
                        "Failed to retrieve states.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<GetStateListDto>>(
                        false,
                        null,
                        "No states found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<GetStateListDto>>(
                    true,
                    response.Data,
                    "states retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving states.");
                await _dbExceptionLogger.LogExceptionAsync("getAllState_Controller", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }

        /// <summary> 18 july 2025
        /// to Get city for drop dow menu
        /// </summary>
        /// <returns>list of countries via GetCityListDto</returns>
        [HttpGet("getAllcities")]
        public async Task<IActionResult> GetAllCity(int?stateId)
        {
            try
            {
                var response = await _listService.GetAllCityAsync(stateId);

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<GetCityListDto>>(
                        false,
                        null,
                        "Failed to retrieve cities.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<GetCityListDto>>(
                        false,
                        null,
                        "No cities found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<GetCityListDto>>(
                    true,
                    response.Data,
                    "cities retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving cities.");
                await _dbExceptionLogger.LogExceptionAsync("GetAllCity_Controller", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }



        /// <summary> 22-july 2025
        /// to Get Vessel type for drop dow menu
        /// </summary>
        /// <returns>list of countries via VesselTypeDto</returns>
        [HttpGet("getAllVesseltype")]
        public async Task<IActionResult> GetAllVesselTypes()
        {
            try
            {
                var response = await _listService.GetAllVesselTypeAsync();

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<VesselTypeDto>>(
                        false,
                        null,
                        "Failed to retrieve vessel types.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<VesselTypeDto>>(
                        false,
                        null,
                        "No vessel types found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<VesselTypeDto>>(
                    true,
                    response.Data,
                    "vessel types retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving vessel types.");
                await _dbExceptionLogger.LogExceptionAsync("getAllVesseltype_Controller", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }        

        /// <summary> 22-july 2025
        /// to Get Contract duration for drop dow menu
        /// </summary>
        /// <returns>list of duration via ContractDurationDto</returns>
        [HttpGet("getAllContractduration")]
        public async Task<IActionResult> GetAllContractDuration()
        {
            try
            {
                var response = await _listService.GetAllContractDurationAsync();

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<ContractDurationDto>>(
                        false,
                        null,
                        "Failed to retrieve Contract Duration.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<ContractDurationDto>>(
                        false,
                        null,
                        "No Contract Duration found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<ContractDurationDto>>(
                    true,
                    response.Data,
                    "Contract Duration retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving Contract Duration.");
                await _dbExceptionLogger.LogExceptionAsync("GetAllContractDuration", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }

        /// <summary> 23 july 2025
        /// to Get position for drop dow menu
        /// </summary>
        /// <returns>list of position via PositionDto</returns>
        [HttpGet("getAllposition")]
        public async Task<IActionResult> GetAllPosition(int companyId)
        {
            try
            {
                var response = await _listService.GetAllPositionAsync(companyId);

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<PositionDto>>(
                        false,
                        null,
                        "Failed to retrieve cities.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<PositionDto>>(
                        false,
                        null,
                        "No cities found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<PositionDto>>(
                    true,
                    response.Data,
                    "cities retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving GetAllPosition.");
                await _dbExceptionLogger.LogExceptionAsync("GetAllPosition", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }


        /// <summary> 18-july 2025
        /// to Get countries for drop dow menu
        /// </summary>
        /// <returns>list of countries via GetCountryListDto</returns>
        [HttpGet("GetAllMobileCodes")]
        public async Task<IActionResult> GetAllMobileCodes()
        {
            try
            {
                var response = await _listService.GetAllMobileCodesAsync();

                if (!response.Success)
                {
                    return StatusCode(500, new ApiResponse<IEnumerable<MobileCountryCodeDto>>(
                        false,
                        null,
                        "Failed to retrieve countries.",
                        response.ErrorCode ?? ErrorCodes.InternalServerError
                    ));
                }


                if (response.Data == null || !response.Data.Any())
                {
                    return NotFound(new ApiResponse<IEnumerable<MobileCountryCodeDto>>(
                        false,
                        null,
                        "No countries found.",
                        ErrorCodes.NotFound
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<MobileCountryCodeDto>>(
                    true,
                    response.Data,
                    "countries retrieved successfully.",
                    ErrorCodes.Success
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving countries.");
                await _dbExceptionLogger.LogExceptionAsync("GetAllCountry_Controller", ex.Message, ex.StackTrace);

                return StatusCode(500, new ApiResponse<string>(
                    false,
                    null,
                    "An unexpected error occurred.",
                    ErrorCodes.InternalServerError
                ));
            }
        }


        ///// <summary> 23 july 2025
        ///// to Get position for drop dow menu
        ///// </summary>
        ///// <returns>list of position via PositionDto</returns>
        //[HttpGet("getdashboardgraphs")]
        //public async Task<IActionResult> GetDashBoardStatistics(int companyId)
        //{
        //    try
        //    {
        //        var response = await _listService.GetAllPositionAsync(companyId);

        //        if (!response.Success)
        //        {
        //            return StatusCode(500, new ApiResponse<IEnumerable<PositionDto>>(
        //                false,
        //                null,
        //                "Failed to retrieve cities.",
        //                response.ErrorCode ?? ErrorCodes.InternalServerError
        //            ));
        //        }


        //        if (response.Data == null || !response.Data.Any())
        //        {
        //            return NotFound(new ApiResponse<IEnumerable<PositionDto>>(
        //                false,
        //                null,
        //                "No cities found.",
        //                ErrorCodes.NotFound
        //            ));
        //        }

        //        return Ok(new ApiResponse<IEnumerable<PositionDto>>(
        //            true,
        //            response.Data,
        //            "cities retrieved successfully.",
        //            ErrorCodes.Success
        //        ));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Exception occurred while retrieving GetAllPosition.");
        //        await _dbExceptionLogger.LogExceptionAsync("GetAllPosition", ex.Message, ex.StackTrace);

        //        return StatusCode(500, new ApiResponse<string>(
        //            false,
        //            null,
        //            "An unexpected error occurred.",
        //            ErrorCodes.InternalServerError
        //        ));
        //    }
        //}
    }
}