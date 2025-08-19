using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Application.Services;

public class GetListService: IGetListService
{
    private readonly IGetListRepository _listRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetListService> _logger;

    public GetListService(IGetListRepository Listservice,IMapper mapper,ILogger<GetListService> logger)
    {
        _listRepository = Listservice;
        _mapper = mapper;
        _logger = logger;

    }

    public async Task<ApiResponse<IEnumerable<GetCountryListDto>>> GetAllCountryAsync()
    {
        try
        {
            var result = await _listRepository.GetAllCountriesAsync();

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<GetCountryListDto>>(result.Data);

                return new ApiResponse<IEnumerable<GetCountryListDto>>(
                    success: true,
                    data: dtoList,
                    message: "countries fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<GetCountryListDto>>(
                success: false,
                data: null,
                message: "No countries found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllCountryAsync");
            throw;
        }
    }


    public async Task<ApiResponse<IEnumerable<GetStateListDto>>> GetAllStateAsync(int? countryId)
    {
        try
        {
            var result = await _listRepository.GetAllStatesAsync(countryId);

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<GetStateListDto>>(result.Data);

                return new ApiResponse<IEnumerable<GetStateListDto>>(
                    success: true,
                    data: dtoList,
                    message: "States fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<GetStateListDto>>(
                success: false,
                data: null,
                message: "No states found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllStateAsync");
            throw;
        }
    }



    public async Task<ApiResponse<IEnumerable<GetCityListDto>>> GetAllCityAsync(int? stateId)
    {
        try
        {
            var result = await _listRepository.GetAllCitiesAsync(stateId);

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<GetCityListDto>>(result.Data);

                return new ApiResponse<IEnumerable<GetCityListDto>>(
                    success: true,
                    data: dtoList,
                    message: "Cities fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<GetCityListDto>>(
                success: false,
                data: null,
                message: "No cities found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllCityAsync");
            throw;
        }
    }


    public async Task<ApiResponse<IEnumerable<VesselTypeDto>>> GetAllVesselTypeAsync()
    {
        try
        {
            var result = await _listRepository.GetAllVesselTypeAsync();

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<VesselTypeDto>>(result.Data);

                return new ApiResponse<IEnumerable<VesselTypeDto>>(
                    success: true,
                    data: dtoList,
                    message: "Vessel type fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<VesselTypeDto>>(
                success: false,
                data: null,
                message: "No vessel types found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllVesselTypeAsync");
            throw;
        }
    }


    public async Task<ApiResponse<IEnumerable<ContractDurationDto>>> GetAllContractDurationAsync()
    {
        try
        {
            var result = await _listRepository.GetAllContractDurationAsync();

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<ContractDurationDto>>(result.Data);

                return new ApiResponse<IEnumerable<ContractDurationDto>>(
                    success: true,
                    data: dtoList,
                    message: "Contract Duration fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<ContractDurationDto>>(
                success: false,
                data: null,
                message: "No Contract Duration found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllContractDurationAsync");
            throw;
        }
    }


    public async Task<ApiResponse<IEnumerable<PositionDto>>> GetAllPositionAsync(int? companyId)
    {
        try
        {
            var result = await _listRepository.GetAllPositionAsync(companyId);

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<PositionDto>>(result.Data);

                return new ApiResponse<IEnumerable<PositionDto>>(
                    success: true,
                    data: dtoList,
                    message: "position fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<PositionDto>>(
                success: false,
                data: null,
                message: "No position found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllPositionAsync");
            throw;
        }
    }


    public async Task<ApiResponse<IEnumerable<MobileCountryCodeDto>>> GetAllMobileCodesAsync()
    {
        try
        {
            var result = await _listRepository.GetAllMobileCountriesAsync();

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<MobileCountryCodeDto>>(result.Data);

                return new ApiResponse<IEnumerable<MobileCountryCodeDto>>(
                    success: true,
                    data: dtoList,
                    message: "mobile country codes fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<MobileCountryCodeDto>>(
                success: false,
                data: null,
                message: "No countries found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllMobileCodesAsync");
            throw;
        }
    }

}
