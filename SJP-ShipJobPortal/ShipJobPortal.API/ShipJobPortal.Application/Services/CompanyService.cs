using AutoMapper;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Domain.Constants;


namespace ShipJobPortal.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CompanyService> _logger;
    public CompanyService(ICompanyRepository companyRepository, IMapper mapper, ILogger<CompanyService> logger)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<string>> CreateCompanyAsync(CompanyCreateDto dto, string username)
    {
        try
        {
            var company = _mapper.Map<CompanyCreate>(dto);
            company.CreatedBy = username;

            var result = await _companyRepository.CreateCompanyAsync(company);

            if (result.ReturnStatus == "success")
            {
                if (dto.CompanyID > 0)
                    return new ApiResponse<string>(true, null, "Updated successfully", ErrorCodes.Success);
                else
                    return new ApiResponse<string>(true, null, "Created successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, "Failed to create or update company", ErrorCodes.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateCompanyAsync");
            throw;
        }
    }



    public async Task<ApiResponse<IEnumerable<CompanyDto>>> GetCompanyAsync(int CompanyId)
    {
        try
        {
            var result = await _companyRepository.GetCompanyAsync(CompanyId);

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<CompanyDto>>(result.Data);

                return new ApiResponse<IEnumerable<CompanyDto>>(
                    success: true,
                    data: dtoList,
                    message: "Companies fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<CompanyDto>>(
                success: false,
                data: null,
                message: "No companies found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllCompaniesAsync");
            throw;
            //return Enumerable.Empty<CompanyDto>(); // Or consider throwing custom exception
        }
    }


    public async Task<ApiResponse<IEnumerable<CompanyDropDto>>> GetAllCompaniesAsync()
    {
        try
        {
            var result = await _companyRepository.GetAllCompaniesAsync();

            if (result.ReturnStatus == "success" && result.Data != null && result.Data.Any())
            {
                var dtoList = _mapper.Map<IEnumerable<CompanyDropDto>>(result.Data);

                return new ApiResponse<IEnumerable<CompanyDropDto>>(
                    success: true,
                    data: dtoList,
                    message: "Companies fetched successfully.",
                    errorCode: ErrorCodes.Success
                );
            }

            return new ApiResponse<IEnumerable<CompanyDropDto>>(
                success: false,
                data: null,
                message: "No companies found.",
                errorCode: result.ErrorCode ?? ErrorCodes.NotFound
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllCompaniesAsync");
            throw;
            //return Enumerable.Empty<CompanyDto>(); // Or consider throwing custom exception
        }
    }

}

