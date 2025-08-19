using Microsoft.AspNetCore.Http;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Application.IServices;

public interface IProfileService
{
    Task<ApiResponse<string>> UploadAndProcessResumeAsync(VideoResumeFileDto file);
    //        Task<ReturnResult> CreateUserProfileAsync(UserProfileDto dto);

    Task<ApiResponse<string>> CreateUserProfileAsync(UserProfileDto dto);

    Task<NationalityDto> CreateNationalityAsync(NationalityDto dto);

    Task<ApiResponse<string>> UserSaveFileAsync(UserResumeandImageDto dto);
    Task<ApiResponse<UserProfileViewDto>> GetSeafarerProfileAsync(int userId);
    Task<ApiResponse<string>> ReferFriendAsync(ReferAFriendDto dto);
    Task<ApiResponse<string>> UpdateUserExperianceAsync(SeaExperianceViewPatchDto dto);
    Task<ApiResponse<string>> UpdateUserDocumentCertificateAsync(CertificatesViewPatchDto dto);
    Task<ApiResponse<CompanyViewDto>> GetcompanyProfileAsync(int userId);
    Task<ApiResponse<string>> GetVideoResumeBase64Async(int userId);
    Task<ApiResponse<UserFilesModel>> GetUserFilesAsync(int userId);


}
