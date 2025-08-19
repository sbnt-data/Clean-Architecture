using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer;

namespace ShipJobPortal.Infrastructure.Repositories;

public class MatchingRepository : IMatchingRepository
{
    private readonly IDataAccess_Improved _dbHelper;
    private readonly IConfiguration _configuration;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<MatchingRepository> _logger;
    private readonly string _dbKey;

    public MatchingRepository(IConfiguration configuration, IDataAccess_Improved dbHelper, IEncryptionService encryptionService, ILogger<MatchingRepository> logger)
    {
        _dbHelper = dbHelper;
        _encryptionService = encryptionService;
        _logger = logger;
        _configuration = configuration;

        _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
            ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
            : "DefaultConnection";
    }

    public async Task<ReturnResult<MatchingCandidateViewModel>> GetMatchingCandidatesAsync(int JobId, int userId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@jobId", JobId);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@positionId", positionId);
            parameters.Add("@vesselTypeId", vesselTypeId);
            parameters.Add("@locationId", locationId);
            parameters.Add("@durationId", durationId);


            var dbResult = await _dbHelper.QueryMultipleAsync("matchdemo", parameters, _dbKey);

            //if (dbResult == null || dbResult.Data.Count < 2)
            //{
            //    return new ReturnResult<MatchingCandidateViewModel>("error", ErrorCodes.InternalServerError, null);
            //}

            // First result set: Candidate list
            var JobList = JsonConvert.DeserializeObject<List<MatchingCandidateModel>>(
                JsonConvert.SerializeObject(dbResult.Data[0])
            );

            // Second result set: Pagination
            //var paginationRaw = dbResult.Data[1].FirstOrDefault();
            //var pagination = paginationRaw != null
            //    ? JsonConvert.DeserializeObject<PaginationModel>(JsonConvert.SerializeObject(paginationRaw))
            //    : new PaginationModel();

            var resultModel = new MatchingCandidateViewModel
            {
                candidates = JobList,
                pagination = new PaginationModel()//pagination
            };

            return new ReturnResult<MatchingCandidateViewModel>(
                dbResult.ReturnStatus ?? "success",
                dbResult.ErrorCode ?? ErrorCodes.Success,
                resultModel
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetMatchingJobAsync");
            throw;
        }
    }

}

