using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface IJobApplyRepository
{
    Task<ReturnResult<AppliedCandidatesListModel>> GetAppliedCandidatesAsync(
int jobId, int pageNumber = 1, int pageSize = 10, int? positionId = 0,
int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, string? searchKey = "");
    Task<ReturnResult<string>> ApplyJobAsync(JobApplyPostModel model);
    Task<ReturnResult<AppliedJobsListModel>> GetAppliedJobsAsync(int UserId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0, string? searchKey = "");
    Task<ReturnResult<string>> WishlistJobAsync(JobWishlistModel model);
    Task<ReturnResult<SavedJobsListModel>> GetSavedJobsAsync(int UserId, int pageNumber = 1, int pageSize = 10, int? positionId = 0, int? vesselTypeId = 0, int? locationId = 0, int? durationId = 0,int?monthValue=0, string? searchKey = "");
    Task<ReturnResult<string>> JobActionOnCandidateAsync(JobActionOnCandidateModel model);
}
