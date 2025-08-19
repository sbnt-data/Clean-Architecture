using System.Data;
using Dapper;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataHelpers;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using AutoMapper;
using ShipJobPortal.Domain.Constants;

namespace ShipJobPortal.Infrastructure.Repositories;

    public class CompanyRepository : ICompanyRepository
    {
        private readonly DataAccess _dbHelper;
        private readonly ILogger<CompanyRepository> _logger;
        private readonly IMapper _mapper;

        public CompanyRepository(DataAccess dbHelper, ILogger<CompanyRepository> logger, IMapper mapper)
        {
            _dbHelper = dbHelper;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ReturnResult> CreateCompanyAsync(CompanyCreate company)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CompanyId", company.CompanyID);

                parameters.Add("@ParentCompanyID", company.ParentCompanyID);
                parameters.Add("@CompanyName", company.CompanyName);
                parameters.Add("@LocalCompanyName", company.LocalCompanyName);
                parameters.Add("@Email", company.Email);
                parameters.Add("@ContactNumber", company.ContactNumber);
                parameters.Add("@Website", company.Website);
                parameters.Add("@Remarks", company.Remarks);
                parameters.Add("@Address", company.Address);
                parameters.Add("@CityID", company.CityID);
                parameters.Add("@CountryID", company.CountryID);
                parameters.Add("@StateID", company.StateID);
                parameters.Add("@PostalCode", company.PostalCode);
                parameters.Add("@IsActive", company.IsActive);

                parameters.Add("@CreatedBy", company.CreatedBy);

                var result = await _dbHelper.QueryAsyncTrans("usp_create_company", parameters);

                return new ReturnResult
                {
                    ReturnStatus = result?.ReturnStatus ?? "error",
                    ErrorCode = result?.ErrorCode ?? ErrorCodes.InternalServerError
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in CreateCompanyAsync");
                throw;
            }
        }
        public async Task<ReturnResult<List<CompanyDropDetails>>> GetAllCompaniesAsync()
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@companyId", 0);
                var result = await _dbHelper.QueryAsyncTrans("usp_get_company_details", parameters);

                var list = result.ListResultset
                         .Select(item => _mapper.Map<CompanyDropDetails>((object)item))
                         .ToList();

                return new ReturnResult<List<CompanyDropDetails>>(result?.ReturnStatus ?? "error", result?.ErrorCode ?? ErrorCodes.InternalServerError, list);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllCompaniesAsync");
                throw;
            }
        }

        public async Task<ReturnResult<List<CompanyDetails>>> GetCompanyAsync(int CompanyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@companyId", CompanyId);
                var result = await _dbHelper.QueryAsyncTrans("usp_get_company_details", parameters);

                var list = result.ListResultset
                         .Select(item => _mapper.Map<CompanyDetails>((object)item))
                         .ToList();

                return new ReturnResult<List<CompanyDetails>>(result?.ReturnStatus ?? "error", result?.ErrorCode ?? ErrorCodes.InternalServerError, list);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetCompanyAsync");
                throw;
            }
        }
    }


