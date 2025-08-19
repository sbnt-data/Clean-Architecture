using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Application.Validators;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;
using static ShipJobPortal.Application.Services.ProfileService;

namespace ShipJobPortal.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly string _resumeApiUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProfileService> _logger;
    private readonly IMapper _mapper;

    public ProfileService(IProfileRepository profileRepository, HttpClient httpClient,IConfiguration configuration, ILogger<ProfileService> logger, IMapper mapper)
    {
        _profileRepository = profileRepository;
        _httpClient = httpClient;
        _resumeApiUrl = configuration.GetValue<string>("ResumeApi");
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiResponse<string>> UploadAndProcessResumeAsync(VideoResumeFileDto file)
    {
        try
        {
            byte[] resumeBytes;

            using (var memoryStream = new MemoryStream())
            {
                await file.resumefile.CopyToAsync(memoryStream);
                resumeBytes = memoryStream.ToArray();
            }

            var entity = new VideoResumeFilesModel
            {
                Userid = file.userId,
                VideoResumeFile = resumeBytes
            };

            var result = await _profileRepository.UserVideoResume(entity);

            if (result.ReturnStatus == "success")
            {
                return new ApiResponse<string>(true, null, "User video resume uploaded successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, "Failed to upload video resume", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading video resume");
            throw;
        }
    }



    private async Task<string> ConvertFileToBase64Async(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public async Task<ApiResponse<string>> CreateUserProfileAsync(UserProfileDto dto)
    {
        try
        {
            if (dto == null)
                return new ApiResponse<string>(false, null, "Invalid user profile input", ErrorCodes.BadRequest);
            var entity = _mapper.Map<UserProfileModel>(dto);

            // ✅ Convert Applicant date fields
            if (entity.applicant != null)
            {
                entity.applicant.Dateofavailability = DateConverter.ConvertToNullableDate(dto.Applicant?.Dateofavailability?.ToString()).ToString();
                entity.applicant.Dateofbirth = DateConverter.ConvertToNullableDate(dto.Applicant?.Dateofbirth?.ToString()).ToString();
            }

            // ✅ Convert each document's issue and expiry date
            if (entity.Documents != null && dto.Documents != null)
            {
                for (int i = 0; i < entity.Documents.Count; i++)
                {
                    entity.Documents[i].Certificateordocumentexpirydate =
                        DateConverter.ConvertToNullableDate(dto.Documents[i].Certificateordocumentexpirydate).ToString();
                    entity.Documents[i].Certificateordocumentissuedate =
                        DateConverter.ConvertToNullableDate(dto.Documents[i].Certificateordocumentissuedate).ToString();
                }
            }

            // ✅ Convert each sea experience's from/to date
            if (entity.Sea_experience != null && dto.Sea_experience != null)
            {
                for (int i = 0; i < entity.Sea_experience.Count; i++)
                {
                    entity.Sea_experience[i].Fromdate =
                        DateConverter.ConvertToNullableDate(dto.Sea_experience[i].Fromdate).ToString();
                    entity.Sea_experience[i].Todate =
                        DateConverter.ConvertToNullableDate(dto.Sea_experience[i].Todate).ToString();
                }
            }

            var result = await _profileRepository.CreateUserProfileAsync(entity);

            if (result.ReturnStatus == "success")
            {
                return new ApiResponse<string>(true, null, "User profile created successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, "Failed to create user profile", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in CreateUserProfileAsync");
            throw;
        }
    }


    public async Task<ApiResponse<string>> UserSaveFileAsync(UserResumeandImageDto dto)
    {
        try
        {
            if (dto == null)
                return new ApiResponse<string>(false, null, "Invalid user file input", ErrorCodes.BadRequest);

            // 🔁 Convert base64 to byte[] here
            byte[]? imageBytes = null;

            if (!string.IsNullOrWhiteSpace(dto.ImageFile))
            {
                try
                {
                    imageBytes = Convert.FromBase64String(dto.ImageFile);
                }
                catch (FormatException)
                {
                    return new ApiResponse<string>(false, null, "Invalid base64 image format", "ERR_INVALID_BASE64_IMAGE");
                }
            }
            byte[]? resumeBytes = null;

            if (!string.IsNullOrWhiteSpace(dto.ResumeFile))
            {
                try
                {
                    resumeBytes = Convert.FromBase64String(dto.ResumeFile);
                }
                catch (FormatException)
                {
                    return new ApiResponse<string>(false, null, "Invalid base64 image format", "ERR_INVALID_BASE64_IMAGE");
                }
            }

            // 🔧 Map to UserFilesModel with converted byte[]
            var entity = new UserFilesModel
            {
                Userid = dto.UserId,
                ResumeFile = resumeBytes,
                ImageFile = imageBytes,
                ImageUrl = dto.ImageUrl
            };

            var result = await _profileRepository.AddUserFilesAsync(entity);

            if (result.ReturnStatus == "success")
            {
                return new ApiResponse<string>(true, null, "User files updated successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, "Failed to update user files", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UserSaveFileAsync");
            throw;
        }
    }

    public async Task<ApiResponse<UserProfileViewDto>> GetSeafarerProfileAsync(int userId)
    {
        try
        {
            var result = await _profileRepository.GetSeafarerProfileAsync(userId);

            if (result.ReturnStatus == "success" && result.ErrorCode == ErrorCodes.Success)
            {
                var dto = _mapper.Map<UserProfileViewDto>(result.Data);
                return new ApiResponse<UserProfileViewDto>(true, dto, "Profile retrieved successfully",ErrorCodes.Success);
            }

            return new ApiResponse<UserProfileViewDto>(false,null, result.ErrorCode, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetSeafarerProfileAsync");
            throw;
        }
    }

    public async Task<ApiResponse<string>> ReferFriendAsync(ReferAFriendDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FriendEmail))
                return new ApiResponse<string>(false, null, "Invalid input", ErrorCodes.BadRequest);

            var model = _mapper.Map<ReferAFriendModel>(dto);
            var result = await _profileRepository.ReferFriendAsync(model);

            if (result.ReturnStatus == "success")
            {
                //here email becuase db insertion is success
                return new ApiResponse<string>(true, null, "Referral successful", result.ErrorCode);
            }

            if (result.ReturnStatus == "already refered by someone")
                return new ApiResponse<string>(false, null, result.ReturnStatus, result.ErrorCode);

            return new ApiResponse<string>(false, null, "Referral failed", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ReferFriendAsync");
            throw;
        }
    }


    public async Task<ApiResponse<string>> UpdateUserExperianceAsync(SeaExperianceViewPatchDto dto)
    {
        try
        {
            // Check if the DTO is null
            if (dto == null)
            {
                return new ApiResponse<string>(false, null, "Invalid user profile input", ErrorCodes.BadRequest);
            }

            // Map DTO to entity
            var entity = _mapper.Map<SeaExperianceViewPatchModel>(dto);

            // ✅ Convert Sea experience's from/to dates if they exist
            if (dto.FromDate != null)
            {
                entity.FromDate = DateConverter.ConvertToNullableDate(dto.FromDate)?.ToString();
            }

            if (dto.ToDate != null)
            {
                entity.ToDate = DateConverter.ConvertToNullableDate(dto.ToDate)?.ToString();
            }

            // ✅ Convert any other date fields
          
            // Proceed to save the updated entity
            var result = await _profileRepository.UpdateSeaExperiance(entity);

            if (result.ReturnStatus == "success")
            {
                return new ApiResponse<string>(true, null, "User experiance updated successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, "Failed to update user profile", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in UpdateUserExperianceAsync");
            throw;
        }
    }


    public async Task<ApiResponse<string>> UpdateUserDocumentCertificateAsync(CertificatesViewPatchDto dto)
    {
        try
        {
            // Check if the DTO is null
            if (dto == null)
            {
                return new ApiResponse<string>(false, null, "Invalid user profile input", ErrorCodes.BadRequest);
            }

            // Map DTO to entity
            var entity = _mapper.Map<CertificatesViewPatchModel>(dto);

            // ✅ Convert Sea experience's from/to dates if they exist
            if (dto.ExpiryDate != null)
            {
                entity.ExpiryDate = DateConverter.ConvertToNullableDate(dto.ExpiryDate)?.ToString();
            }

            if (dto.IssuedDate != null)
            {
                entity.IssuedCountry = DateConverter.ConvertToNullableDate(dto.IssuedCountry)?.ToString();
            }

            // ✅ Convert any other date fields

            // Proceed to save the updated entity
            var result = await _profileRepository.UpdateCertificatesAsync(entity);

            if (result.ReturnStatus == "success")
            {
                return new ApiResponse<string>(true, null, "User certificate updated successfully", ErrorCodes.Success);
            }

            return new ApiResponse<string>(false, null, "Failed to update user certificate", result.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in UpdateUserDocumentCertificateAsync");
            throw;
        }
    }





    public async Task<NationalityDto> CreateNationalityAsync(NationalityDto dto)
    {
        try
        {
            var model = _mapper.Map<NationalityModel>(dto);
            var result = await _profileRepository.CreateNationalityAsync(model);
            return _mapper.Map<NationalityDto>(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CreateNationalityAsync");
            throw;
        }
    }


    public async Task<ApiResponse<CompanyViewDto>> GetcompanyProfileAsync(int companyId)
    {
        try
        {
            var result = await _profileRepository.fn_GetcompanyProfileAsync(companyId);

            if (result.ReturnStatus == "success" && result.ErrorCode == ErrorCodes.Success)
            {
                var dto = _mapper.Map<CompanyViewDto>(result.Data);
                return new ApiResponse<CompanyViewDto>(true, dto, "Company Profile retrieved successfully", ErrorCodes.Success);
            }

            return new ApiResponse<CompanyViewDto>(false, null, result.ErrorCode, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetcompanyProfileAsync");
            throw;
        }
    }


    public async Task<ApiResponse<string>> GetVideoResumeBase64Async(int userId)
    {
        try
        {
            var res = await _profileRepository.GetVideoResumeAsync(userId); // ReturnResult<VideoResumeFilesModel>
            var bytes = res.Data?.VideoResumeFile;

            if (res.ReturnStatus != "success" || bytes == null || bytes.Length == 0)
                return new ApiResponse<string>(false, null, "No video resume found", "ERR404");

            var base64 = Convert.ToBase64String(bytes);
            var dataUrl = $"data:video/webm;base64,{base64}";
            return new ApiResponse<string>(true, dataUrl, "OK", "200");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in GetVideoResumeBase64Async");
            throw;
        }
    }



    public async Task<ApiResponse<UserFilesModel>> GetUserFilesAsync(int userId)
    {

        try
        {
            var result = await _profileRepository.GetUserFiles(userId); // your method from the question

            var status = result?.ReturnStatus ?? "error";
            var code = result?.ErrorCode ?? ErrorCodes.InternalServerError;

            // Success
            if (status.Equals("success", StringComparison.OrdinalIgnoreCase) &&
                (code == ErrorCodes.Success || code == "ERR200"))
            {
                if (result!.Data is not null)
                    return new ApiResponse<UserFilesModel>(true, result.Data, "Fetched user files.", code);

                // edge case: success but no data
                return new ApiResponse<UserFilesModel>(false, null, "User files not found.", "ERR404");
            }

            // Not found
            if (status.Equals("not_found", StringComparison.OrdinalIgnoreCase) || result?.Data is null)
                return new ApiResponse<UserFilesModel>(false, null, "User files not found.", "ERR404");

            // Any other failure from repo
            return new ApiResponse<UserFilesModel>(false, null, "Failed to fetch user files.", code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserFilesAsync for UserId={UserId}", userId);
            throw;
        }
    }
    }



