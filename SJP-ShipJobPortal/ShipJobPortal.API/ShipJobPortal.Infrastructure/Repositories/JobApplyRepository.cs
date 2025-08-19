using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
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

    public class JobApplyRepository: IJobApplyRepository
    {
        private readonly IDataAccess_Improved _dbHelper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JobPostRepository> _logger;
        private readonly string _dbKey;

        public JobApplyRepository(IConfiguration configuration, IDataAccess_Improved dbHelper, ILogger<JobPostRepository> logger)
        {
            _configuration = configuration;
            _dbHelper = dbHelper;
            _logger = logger;

            _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
                ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
                : "DefaultConnection";
        }


        public async Task<ReturnResult<AppliedCandidatesListModel>> GetAppliedCandidatesAsync(
    int jobId, int pageNumber = 1, int pageSize = 10, int? positionId = 0,
int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, string? searchKey = "")
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@JobId", jobId);
                parameters.Add("@UserId", 0);
                parameters.Add("@Role", "recruiter");
                parameters.Add("@PageNumber", pageNumber);
                parameters.Add("@PageSize", pageSize);
                parameters.Add("@positionId", positionId);
                parameters.Add("@vesselTypeId", vesselTypeId);
                parameters.Add("@locationId", locationId);
                parameters.Add("@durationId", durationId);
                parameters.Add("@searchKey", searchKey);

                var dbResult = await _dbHelper.QueryMultipleAsync("usp_get_jobApplied_history_test1", parameters,_dbKey);

                if (dbResult == null || dbResult.Data.Count < 2)
                {
                    return new ReturnResult<AppliedCandidatesListModel>("error", ErrorCodes.InternalServerError, null);
                }

                // First result set: Candidate list
                var candidateList = JsonConvert.DeserializeObject<List<JobAppliedModel>>(
                    JsonConvert.SerializeObject(dbResult.Data[0])
                );

                // Second result set: Pagination
                var paginationRaw = dbResult.Data[1].FirstOrDefault();
                var pagination = paginationRaw != null
                    ? JsonConvert.DeserializeObject<PaginationModel>(JsonConvert.SerializeObject(paginationRaw))
                    : new PaginationModel();

                var resultModel = new AppliedCandidatesListModel
                {
                    CandidatesList = candidateList,
                    pagination = pagination
                };

                return new ReturnResult<AppliedCandidatesListModel>(
                    dbResult.ReturnStatus ?? "success",
                    dbResult.ErrorCode ?? ErrorCodes.Success,
                    resultModel
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetAppliedCandidatesAsync");
                throw;            }
        }


        public async Task<ReturnResult<string>> ApplyJobAsync(JobApplyPostModel model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@JobId", model.JobId);
                parameters.Add("@CoverLetter", model.CoverLetter); // Replace with actual value if needed
                parameters.Add("@ResumeFile", model.ResumeFile);  // Replace with actual value if needed
                parameters.Add("@Notes", model.Notes);       // Replace with actual value if needed

                parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);

                // 🔄 Get DbResult from your shared ExecuteAsync
                var dbResult = await _dbHelper.ExecuteScalarAsync("usp_insert_job_application", parameters, _dbKey);

                // 🔁 Map to ReturnResult<int>
                var result = new ReturnResult<string>
                {
                    ReturnStatus = dbResult.ReturnStatus,
                    ErrorCode = dbResult.ErrorCode,
                    Data = null // usually 0 from ExecuteAsync unless rows affected
                };

                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
        }


        public async Task<ReturnResult<AppliedJobsListModel>> GetAppliedJobsAsync(
int UserId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, 
int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, string? searchKey = "")
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@JobId", 0);
                parameters.Add("@UserId", UserId);
                parameters.Add("@Role", "Seafarer");
                parameters.Add("@PageNumber", pageNumber);
                parameters.Add("@PageSize", pageSize);
                parameters.Add("@positionId", positionId);
                parameters.Add("@vesselTypeId", vesselTypeId);
                parameters.Add("@locationId", locationId);
                parameters.Add("@durationId", durationId);
                parameters.Add("@searchKey", searchKey);

            var dbResult = await _dbHelper.QueryMultipleAsync("usp_get_jobApplied_history_test1", parameters,_dbKey);

                if (dbResult == null || dbResult.Data.Count < 2)
                {
                    return new ReturnResult<AppliedJobsListModel>("error", ErrorCodes.InternalServerError, null);
                }

                // First result set: Candidate list
                var JobList = JsonConvert.DeserializeObject<List<AppliedJobs>>(
                    JsonConvert.SerializeObject(dbResult.Data[0])
                );

                // Second result set: Pagination
                var paginationRaw = dbResult.Data[1].FirstOrDefault();
                var pagination = paginationRaw != null
                    ? JsonConvert.DeserializeObject<PaginationModel>(JsonConvert.SerializeObject(paginationRaw))
                    : new PaginationModel();

                var resultModel = new AppliedJobsListModel
                {
                    JobsList = JobList,
                    pagination = pagination
                };

                return new ReturnResult<AppliedJobsListModel>(
                    dbResult.ReturnStatus ?? "success",
                    dbResult.ErrorCode ?? ErrorCodes.Success,
                    resultModel
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetAppliedCandidatesAsync");
                throw;
            }
        }


        public async Task<ReturnResult<string>> WishlistJobAsync(JobWishlistModel model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@JobId", model.JobId);

                // 🔄 Get DbResult from your shared ExecuteAsync
                var dbResult = await _dbHelper.ExecuteScalarAsync("usp_save_candidate_job_wishlist", parameters, _dbKey);

                // 🔁 Map to ReturnResult<int>
                var result = new ReturnResult<string>
                {
                    ReturnStatus = dbResult.ReturnStatus,
                    ErrorCode = dbResult.ErrorCode,
                    Data = null // usually 0 from ExecuteAsync unless rows affected
                };

                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
        }


        public async Task<ReturnResult<SavedJobsListModel>> GetSavedJobsAsync(
int UserId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, int? monthValue = 0, string? searchKey = "")
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", UserId);
                parameters.Add("@PageNumber", pageNumber);
                parameters.Add("@PageSize", pageSize);
                parameters.Add("@positionId", positionId);
                parameters.Add("@vesselTypeId", vesselTypeId);
                parameters.Add("@locationId", locationId);
                parameters.Add("@durationId", durationId);
                parameters.Add("@durationId", monthValue);
                parameters.Add("@searchKey", searchKey);


                var dbResult = await _dbHelper.QueryMultipleAsync("usp_get_user_saved_jobs_test1", parameters,_dbKey);

                if (dbResult == null || dbResult.Data.Count < 2)
                {
                    return new ReturnResult<SavedJobsListModel>("error", ErrorCodes.InternalServerError, null);
                }

                // First result set: Candidate list
                var JobList = JsonConvert.DeserializeObject<List<SavedJobsModel>>(
                    JsonConvert.SerializeObject(dbResult.Data[0])
                );

                // Second result set: Pagination
                var paginationRaw = dbResult.Data[1].FirstOrDefault();
                var pagination = paginationRaw != null
                    ? JsonConvert.DeserializeObject<PaginationModel>(JsonConvert.SerializeObject(paginationRaw))
                    : new PaginationModel();

                var resultModel = new SavedJobsListModel
                {
                    JobsList = JobList,
                    pagination = pagination
                };

                return new ReturnResult<SavedJobsListModel>(
                    dbResult.ReturnStatus ?? "success",
                    dbResult.ErrorCode ?? ErrorCodes.Success,
                    resultModel
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetSavedJobsAsync");
                throw;            
            }
        }

    public async Task<ReturnResult<string>> JobActionOnCandidateAsync(JobActionOnCandidateModel model)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@candidateId", model.candidateId);
            parameters.Add("@jobAction", model.jobAction); 
            parameters.Add("@applicationId", model.applicationId);

            // 🔄 Get DbResult from your shared ExecuteAsync
            var dbResult = await _dbHelper.ExecuteScalarAsync("usp_update_job_application_status", parameters, _dbKey);

            // 🔁 Map to ReturnResult<int>
            var result = new ReturnResult<string>
            {
                ReturnStatus = dbResult.ReturnStatus,
                ErrorCode = dbResult.ErrorCode,
                Data = null // usually 0 from ExecuteAsync unless rows affected
            };

            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }


}

