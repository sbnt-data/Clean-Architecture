using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer;
using ShipJobPortal.Infrastructure.DataHelpers;

namespace ShipJobPortal.Infrastructure.Repositories;

    public class GetListRepository : IGetListRepository
    {

        private readonly IDataAccess_Improved _dbHelper;
        private readonly ILogger<GetListRepository> _logger;
        private readonly IMapper _mapper;
        private readonly string _dbKey;
        private readonly IConfiguration _configuration;


        public GetListRepository(IConfiguration configuration, IDataAccess_Improved dbHelper,ILogger<GetListRepository> logger, IMapper mapper)
        {
            _dbHelper = dbHelper;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;

            _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
                ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
                : "DefaultConnection";
        }

        public async Task<ReturnResult<List<GetCountryList>>> GetAllCountriesAsync()
        {
            try
            {
                var parameters = new DynamicParameters();

                var dbResult = await _dbHelper.QueryAsyncTrans<GetCountryList>("usp_Get_country_list", parameters,_dbKey);

                return new ReturnResult<List<GetCountryList>>(
                    returnStatus: dbResult?.ReturnStatus ?? "error",
                    errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                    data: dbResult?.Data ?? new List<GetCountryList>()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllCountriesAsync");
                throw;
            }
        }


        public async Task<ReturnResult<List<GetStateList>>> GetAllStatesAsync(int? countryId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@countryId", countryId ?? 0);

                var dbResult = await _dbHelper.QueryAsyncTrans<GetStateList>("usp_Get_State_list", parameters, _dbKey);

                return new ReturnResult<List<GetStateList>>(
                    returnStatus: dbResult?.ReturnStatus ?? "error",
                    errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                    data: dbResult?.Data ?? new List<GetStateList>()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllStatesAsync");
                throw;
            }
        }

        public async Task<ReturnResult<List<GetCityList>>> GetAllCitiesAsync(int? stateId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@stateId", stateId ?? 0);

                var dbResult = await _dbHelper.QueryAsyncTrans<GetCityList>("usp_Get_City_list", parameters, _dbKey);

                return new ReturnResult<List<GetCityList>>(
                    returnStatus: dbResult?.ReturnStatus ?? "error",
                    errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                    data: dbResult?.Data ?? new List<GetCityList>()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllCitiesAsync");
                throw;
            }
        }

        public async Task<ReturnResult<List<VesselTypeDrop>>> GetAllVesselTypeAsync()
        {
            try
            {
                var parameters = new DynamicParameters();

                var dbResult = await _dbHelper.QueryAsyncTrans<VesselTypeDrop>("usp_Get_Vessel_Type_list", parameters, _dbKey);

                return new ReturnResult<List<VesselTypeDrop>>(
                    returnStatus: dbResult?.ReturnStatus ?? "error",
                    errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                    data: dbResult?.Data ?? new List<VesselTypeDrop>()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllVesselTypeAsync");
                throw;
            }
        }

        public async Task<ReturnResult<List<ContractDurationDrop>>> GetAllContractDurationAsync()
        {
            try
            {
                var parameters = new DynamicParameters();

                var dbResult = await _dbHelper.QueryAsyncTrans<ContractDurationDrop>("usp_Get_ContractMonths_list", parameters, _dbKey);

                return new ReturnResult<List<ContractDurationDrop>>(
                    returnStatus: dbResult?.ReturnStatus ?? "error",
                    errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                    data: dbResult?.Data ?? new List<ContractDurationDrop>()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllContractDurationAsync");
                throw;
            }
        }
        public async Task<ReturnResult<List<PositionDrop>>> GetAllPositionAsync(int?companyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@companyId", companyId ?? 0);

                var dbResult = await _dbHelper.QueryAsyncTrans<PositionDrop>("usp_Get_Position_list", parameters, _dbKey);

                return new ReturnResult<List<PositionDrop>>(
                    returnStatus: dbResult?.ReturnStatus ?? "error",
                    errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                    data: dbResult?.Data ?? new List<PositionDrop>()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllContractDurationAsync");
                throw;
            }
        }

    public async Task<ReturnResult<List<MobileCountryCodeModel>>> GetAllMobileCountriesAsync()
    {
        try
        {
            var parameters = new DynamicParameters();

            var dbResult = await _dbHelper.QueryAsyncTrans<MobileCountryCodeModel>("usp_GetCountryCodesDropdown", parameters, _dbKey);

            return new ReturnResult<List<MobileCountryCodeModel>>(
                returnStatus: dbResult?.ReturnStatus ?? "error",
                errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                data: dbResult?.Data ?? new List<MobileCountryCodeModel>()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in GetAllCountriesAsync");
            throw;
        }
    }

}

