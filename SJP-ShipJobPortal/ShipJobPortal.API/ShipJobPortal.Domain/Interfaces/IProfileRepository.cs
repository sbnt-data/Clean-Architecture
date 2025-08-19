using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface IProfileRepository
{
    Task<ReturnResult> CreateUserProfileAsync(UserProfileModel userProfile);
    Task<ReturnResult> AddUserFilesAsync(UserFilesModel Userfiles);
    Task<ReturnResult> UpdateSeaExperiance(SeaExperianceViewPatchModel experiance);
    Task<ReturnResult> UpdateCertificatesAsync(CertificatesViewPatchModel certificate);
    Task<ReturnResult<UserProfileViewModel>> GetSeafarerProfileAsync(int userId);
    Task<ReturnResult<string>> ReferFriendAsync(ReferAFriendModel model);
    Task<ReturnResult> UserVideoResume(VideoResumeFilesModel model);
    Task<ReturnResult<CompanyViewModel>> fn_GetcompanyProfileAsync(int companyId);
    Task<ReturnResult<VideoResumeFilesModel>> GetVideoResumeAsync(int userId);

    Task<object> CreateNationalityAsync(NationalityModel nationality);

    Task<ReturnResult<UserFilesModel>> GetUserFiles(int userId);


}