using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface IJobPostRepository
{
    Task<ReturnResult> CreateorUpdateJobAsync(JobPostModel job);
    Task<ReturnResult<JobViewFilteredModel>> GetAllJobsFilteredAsync(int? vacancyId, int? userId, int? companyId, int CandidateId, int ?pageNumber=1, int ?pageSize=7,int? positionId = 0, int? vesselTypeId = 0,int? locationId = 0, int? durationId = 0,string? searchKey = "");
    Task<ReturnResult> DeleteJobAsync(int vacancyId);
    Task<ReturnResult<string>> JobViewCountAsync(JobViewCountModel model);
    //Task<ReturnResult<IEnumerable<JobPostModel>>> GetJobByIdAsync(int vacancyId);

    //Task<ReturnResult<IEnumerable<JobViewModel>>> GetAllJobsFilteredAsync(int? userId, int? companyId, int pageNumber, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, string? searchKey = "");

}
