
using System.ComponentModel.Design;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataAccessLayer;

namespace ShipJobPortal.Infrastructure.Repositories;

    public class JobPostRepository : IJobPostRepository
    {
        private readonly IDataAccess_Improved _dbHelper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JobPostRepository> _logger;
        private readonly string _dbKey;

        public JobPostRepository(IConfiguration configuration, IDataAccess_Improved dbHelper, ILogger<JobPostRepository> logger)
        {
            _configuration = configuration;
            _dbHelper = dbHelper;
            _logger = logger;

            _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
                ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
                : "DefaultConnection";
        }

        public async Task<ReturnResult> CreateorUpdateJobAsync(JobPostModel job)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@VacancyId", job.JobId);
                parameters.Add("@CompanyId", job.CompanyId);
                parameters.Add("@JobTitle", job.JobTitle);
                parameters.Add("@NoVacancy", job.NoVacancy);
                parameters.Add("@NationalityId", job.NationalityId);
                parameters.Add("@Note", job.JobDescription);
                parameters.Add("@OpenDate", job.OpenDate);
                parameters.Add("@CloseDate", job.CloseDate);
                parameters.Add("@MinAge", job.MinAge);
                parameters.Add("@MaxAge", job.MaxAge);
                parameters.Add("@SendNotification", job.SendNotification);
                parameters.Add("@PublishTo", string.Empty);
                parameters.Add("@PublishedDate", job.PublishedDate);
                parameters.Add("@Status", job.Status);
                parameters.Add("@userId", job.UserId);
                parameters.Add("@salary", job.Salary);
                parameters.Add("@vesselType", job.vesselTypeId);
                parameters.Add("@position", job.PositionId);
                parameters.Add("@location", job.LocationId);
                parameters.Add("@preferedLocation", job.PreferedLocationId);
                parameters.Add("@duration", job.DurationId);
                parameters.Add("@Mode", job.JobId > 0
                    ? _configuration["StoredProcedureModes:Update"]
                    : _configuration["StoredProcedureModes:Insert"]);

                await _dbHelper.ExecuteScalarAsync("usp_JobPosting_Create", parameters, _dbKey);


                return new ReturnResult
                {
                    ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                    ErrorCode = parameters.Get<string>("@ErrorCode")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating/updating the job vacancy");
                throw;
            }
        }

        public async Task<ReturnResult<JobViewFilteredModel>> GetAllJobsFilteredAsync(
    int? vacancyId, int? userId, int? companyId, int CandidateId, int ?pageNumber=1, int ?pageSize=7,
    int? positionId = 0, int? vesselTypeId = 0,
    int? locationId = 0, int? durationId = 0,
    string? searchKey = "")
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@VacancyId", vacancyId);
                parameters.Add("@UserId", userId);
                parameters.Add("@companyId", companyId);
                parameters.Add("@positionId", positionId);
                parameters.Add("@vesselTypeId", vesselTypeId);
                parameters.Add("@locationId", locationId);
                parameters.Add("@durationId", durationId);
                parameters.Add("@searchKey", searchKey);
                parameters.Add("@PageNumber", pageNumber);
                parameters.Add("@PageSize", pageSize);
                parameters.Add("@CandidateId",CandidateId);

                var dbResult = await _dbHelper.QueryMultipleAsync("usp_Job_details_test1", parameters,_dbKey);

                if (dbResult == null || dbResult.Data.Count < 2)
                {
                    return new ReturnResult<JobViewFilteredModel>("error", ErrorCodes.InternalServerError, null);
                }

                // First Result Set: Jobs List
                var jobList = JsonConvert.DeserializeObject<List<JobViewModel>>(
                    JsonConvert.SerializeObject(dbResult.Data[0])
                );

                // Second Result Set: Pagination Info
                var paginationRaw = dbResult.Data[1].FirstOrDefault();
                var pagination = paginationRaw != null
                    ? JsonConvert.DeserializeObject<PaginationModel>(JsonConvert.SerializeObject(paginationRaw))
                    : new PaginationModel(); 

                // Combine into the final result model
                var resultModel = new JobViewFilteredModel
                {
                    JobsList = jobList,
                    Pagination = pagination
                };

                return new ReturnResult<JobViewFilteredModel>(
                    returnStatus: dbResult.ReturnStatus ?? "success",
                    errorCode: dbResult.ErrorCode ?? ErrorCodes.Success,
                    data: resultModel
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetAllJobsFilteredAsync");
                throw;
            }
        }

        public async Task<ReturnResult<string>> JobViewCountAsync(JobViewCountModel model)
        {
            var result = new ReturnResult<string>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", model.userid);
                parameters.Add("@JobId", model.jobid);


                await _dbHelper.ExecuteScalarAsync("usp_Insert_JobViewHistory", parameters, _dbKey);

                result.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                result.ErrorCode = parameters.Get<string>("@ErrorCode");
                result.Data = result.ReturnStatus;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //public async Task<ReturnResult<JobViewFilteredModel>> GetAllJobsFilteredAsync(int?vacancyId,
        //    int? userId, int? companyId, int pageNumber,
        //    int? positionId = 0, int? vesselTypeId = 0,
        //    int? locationId = 0, int? durationId = 0,
        //    string? searchKey = "")
        //{
        //    try
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@VacancyId", vacancyId);
        //        parameters.Add("@UserId", userId);
        //        parameters.Add("@companyId", companyId);
        //        parameters.Add("@positionId", positionId);
        //        parameters.Add("@vesselTypeId", vesselTypeId);
        //        parameters.Add("@locationId", locationId);
        //        parameters.Add("@durationId", durationId);
        //        parameters.Add("@searchKey", searchKey);
        //        parameters.Add("@PageNumber", pageNumber);
        //        parameters.Add("@PageSize", 7);

        //        var dbResult = await _dbHelper.QueryMultipleAsync("usp_Job_details_test1", parameters);

        //        if (dbResult == null || dbResult.Data.Count < 2)
        //        {
        //            return new ReturnResult<JobViewFilteredModel>("error", ErrorCodes.InternalServerError, null);
        //        }

        //        var jobList = JsonConvert.DeserializeObject<List<JobViewModel>>(
        //     JsonConvert.SerializeObject(dbResult.Data[0])
        // );

        //        var pagination = dbResult.Data[1].FirstOrDefault() != null
        //            ? JsonConvert.DeserializeObject<JobViewFilteredModel>(JsonConvert.SerializeObject(dbResult.Data[1].FirstOrDefault()))
        //            : new JobViewFilteredModel();

        //        pagination.JobsList = jobList;

        //        return new ReturnResult<JobViewFilteredModel>(
        //            returnStatus: dbResult.ReturnStatus ?? "success",
        //            errorCode: dbResult.ErrorCode ?? ErrorCodes.Success,
        //            data: pagination
        //        );

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Exception in GetAllJobsFilteredAsync");
        //        return new ReturnResult<JobViewFilteredModel>("error", ErrorCodes.InternalServerError, null);
        //    }
        //}

        public async Task<ReturnResult> DeleteJobAsync(int vacancyId)
        {
            try
            {
                JobPostModel job = new();
                var parameters = new DynamicParameters();
                parameters.Add("@VacancyId", vacancyId);
                parameters.Add("@Mode", _configuration["StoredProcedureModes:Delete"]);
                parameters.Add("@CompanyId", job.CompanyId);
                parameters.Add("@JobTitle", job.JobTitle);
                parameters.Add("@NoVacancy", job.NoVacancy);
                parameters.Add("@NationalityId", job.NationalityId);
                parameters.Add("@Note", job.JobDescription);
                parameters.Add("@OpenDate", job.OpenDate);
                parameters.Add("@CloseDate", job.CloseDate);
                parameters.Add("@MinAge", job.MinAge);
                parameters.Add("@MaxAge", job.MaxAge);
                parameters.Add("@SendNotification", job.SendNotification);
                parameters.Add("@PublishTo", string.Empty);
                parameters.Add("@PublishedDate", job.PublishedDate);
                parameters.Add("@Status", job.Status);
                parameters.Add("@userId", job.UserId);
                parameters.Add("@salary", job.Salary);
                parameters.Add("@vesselType", job.vesselTypeId);
                parameters.Add("@position", job.PositionId);
                parameters.Add("@location", job.NationalityId);
                parameters.Add("@duration", job.DurationId);
                parameters.Add("@preferedLocation", job.PreferedLocationId);



                await _dbHelper.ExecuteScalarAsync("usp_JobPosting_Create", parameters, _dbKey);

                return new ReturnResult
                {
                    ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                    ErrorCode = parameters.Get<string>("@ErrorCode")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the job vacancy");
                throw;
            }
        }

    }







/*
 
        public async Task<ReturnResult<IEnumerable<JobPostModel>>> GetJobByIdAsync(int vacancyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@VacancyId", vacancyId);
                parameters.Add("@UserId", 0);
                parameters.Add("@companyId", 0);
                parameters.Add("@positionId", 0);
                parameters.Add("@vesselTypeId", 0);
                parameters.Add("@locationId", 0);
                parameters.Add("@durationId", 0);
                parameters.Add("@searchKey", null);
                parameters.Add("@PageNumber", 0);
                parameters.Add("@PageSize", 0);
                parameters.Add("@CandidateId", 0);
                var dbResult = await _dbHelper.QueryAsyncTrans<JobPostModel>("usp_Job_details_test1", parameters, _dbKey);

                if (dbResult != null && dbResult.ReturnStatus == "success")
                {
                    return new ReturnResult<IEnumerable<JobPostModel>>(
                        returnStatus: "success",
                        errorCode: dbResult.ErrorCode ?? ErrorCodes.Success,
                        data: dbResult.Data ?? new List<JobPostModel>()
                    );
                }
                else if (dbResult != null && dbResult.Data != null && dbResult.Data.Any())
                {
                    // Fallback in case ReturnStatus was not set but data exists
                    return new ReturnResult<IEnumerable<JobPostModel>>(
                        returnStatus: "success",
                        errorCode: ErrorCodes.Success,
                        data: dbResult.Data
                    );
                }
                else
                {
                    return new ReturnResult<IEnumerable<JobPostModel>>(
                        returnStatus: dbResult?.ReturnStatus ?? "error",
                        errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                        data: dbResult?.Data ?? new List<JobPostModel>()
                    );
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllJobsAsync");
                throw;
            }
        }

        public async Task<ReturnResult<IEnumerable<JobViewModel>>> GetAllJobsAsync(int? vacancyId = 0)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@VacancyId", vacancyId);
                parameters.Add("@UserId", 0);
                parameters.Add("@companyId", 0);
                var dbResult = await _dbHelper.QueryAsyncTrans<JobViewModel>("usp_Job_details", parameters);

                if (dbResult != null && dbResult.ReturnStatus == "success")
                {
                    return new ReturnResult<IEnumerable<JobViewModel>>(
                        returnStatus: "success",
                        errorCode: dbResult.ErrorCode ?? ErrorCodes.Success,
                        data: dbResult.Data ?? new List<JobViewModel>()
                    );
                }
                else if (dbResult != null && dbResult.Data != null && dbResult.Data.Any())
                {
                    // Fallback in case ReturnStatus was not set but data exists
                    return new ReturnResult<IEnumerable<JobViewModel>>(
                        returnStatus: "success",
                        errorCode: ErrorCodes.Success,
                        data: dbResult.Data
                    );
                }
                else
                {
                    return new ReturnResult<IEnumerable<JobViewModel>>(
                        returnStatus: dbResult?.ReturnStatus ?? "error",
                        errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                        data: dbResult?.Data ?? new List<JobViewModel>()
                    );
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllJobsAsync");
                return new ReturnResult<IEnumerable<JobViewModel>>("error", ErrorCodes.InternalServerError, new List<JobViewModel>());
            }
        }

        public async Task<ReturnResult<IEnumerable<JobViewModel>>> GetAllJobsbyUserAsync(int? userId,int?companyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@VacancyId", -1);
                parameters.Add("@UserId", userId);
                parameters.Add("@companyId", companyId);

                var dbResult = await _dbHelper.QueryAsyncTrans<JobViewModel>("usp_Job_details", parameters);

                if (dbResult != null && dbResult.ReturnStatus == "success")
                {
                    return new ReturnResult<IEnumerable<JobViewModel>>(
                        returnStatus: "success",
                        errorCode: dbResult.ErrorCode ?? ErrorCodes.Success,
                        data: dbResult.Data ?? new List<JobViewModel>()
                    );
                }
                else if (dbResult != null && dbResult.Data != null && dbResult.Data.Any())
                {
                    // Fallback in case ReturnStatus was not set but data exists
                    return new ReturnResult<IEnumerable<JobViewModel>>(
                        returnStatus: "success",
                        errorCode: ErrorCodes.Success,
                        data: dbResult.Data
                    );
                }
                else
                {
                    return new ReturnResult<IEnumerable<JobViewModel>>(
                        returnStatus: dbResult?.ReturnStatus ?? "error",
                        errorCode: dbResult?.ErrorCode ?? ErrorCodes.InternalServerError,
                        data: dbResult?.Data ?? new List<JobViewModel>()
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllJobsAsync");
                return new ReturnResult<IEnumerable<JobViewModel>>("error", ErrorCodes.InternalServerError, new List<JobViewModel>());
            }
        }



using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using ShipJobPortal.Infrastructure.DataHelpers;

namespace ShipJobPortal.Infrastructure.Repositories
{
    public class JobPostRepository : IJobPostRepository
    {
        private readonly DataAccess _dbHelper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JobPostRepository> _logger;

        public JobPostRepository(IConfiguration configuration, DataAccess dbHelper, ILogger<JobPostRepository> logger)
        {
            _configuration = configuration;
            _dbHelper = dbHelper;
            _logger = logger;
        }

        public async Task<object> CreateJobAsync(JobPostModel job)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@VacancyId", job.VacancyId);
                parameters.Add("@CompanyId", job.CompanyId);
                parameters.Add("@JobTitle", job.JobTitle);
                parameters.Add("@NoVacancy", job.NoVacancy);
                parameters.Add("@NationalityId", job.NationalityId);
                parameters.Add("@Note", job.Note);
                parameters.Add("@OpenDate", job.OpenDate);
                parameters.Add("@CloseDate", job.CloseDate);
                parameters.Add("@MinAge", job.MinAge);
                parameters.Add("@MaxAge", job.MaxAge);
                parameters.Add("@SendNotification", job.SendNotification);
                parameters.Add("@PublishTo", job.PublishTo);
                parameters.Add("@PublishedDate", job.PublishedDate);
                parameters.Add("@Status", job.Status);

                parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
                parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

                parameters.Add("@Mode", job.VacancyId > 0
                    ? _configuration["StoredProcedureModes:Update"]
                    : _configuration["StoredProcedureModes:Insert"]);

                var result = await _dbHelper.QueryAsyncTrans("JobPosting_Create", parameters);

                return new ReturnResult
                {
                    ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                    ErrorCode = parameters.Get<string>("@ErrorCode")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the job vacancy");
                throw;
            }
        }

        public async Task<IEnumerable<JobPostModel>> GetAllJobsAsync(int? vacancyId = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@VacancyId", vacancyId);
                var result = await _dbHelper.QueryAsyncTrans("usp_Job_details", parameters);

                return new ReturnResult<IEnumerable<JobPostModel>>("sucess",ErrorCodes.Success,result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllJobsAsync");
                throw;
            }
        }

        public async Task<ReturnResult> DeleteJobAsync(int vacancyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", _configuration["StoredProcedureModes:Delete"]);
                parameters.Add("@VacancyId", vacancyId);
                parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
                parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

                var result = await _dbHelper.QueryAsyncTrans("JobPosting_Create", parameters);

                return new ReturnResult
                {
                    ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                    ErrorCode = parameters.Get<string>("@ErrorCode")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the job vacancy");
                throw;
            }
        }
    }
}
*/