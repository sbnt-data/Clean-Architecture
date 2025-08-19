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
using ShipJobPortal.Infrastructure.DataHelpers;
using ShipJobPortal.Infrastructure.Helpers;

namespace ShipJobPortal.Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly IDataAccess_Improved _dbHelper;
    private readonly IConfiguration _configuration;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<ProfileRepository> _logger;
    private readonly string _dbKey;

    public ProfileRepository(IConfiguration configuration, IDataAccess_Improved dbHelper, IEncryptionService encryptionService, ILogger<ProfileRepository> logger)
    {
        _dbHelper = dbHelper;
        _encryptionService = encryptionService;
        _logger = logger;
        _configuration = configuration;

        _dbKey = string.IsNullOrWhiteSpace(_configuration["ConnectionStrings:DefaultConnection"])
            ? throw new Exception("DefaultConnection is missing in ConnectionStrings.")
            : "DefaultConnection";
    }


    private DataTable ToDocumentsTable(List<DocumentsAndCertications> documents)
    {
        try
        {
            var dt = new DataTable();
            dt.Columns.Add("certificateORdocumentExpiryDate", typeof(DateTime));
            dt.Columns.Add("certificateORdocumentIssueDate", typeof(DateTime));
            dt.Columns.Add("certificateORdocumentIssuingCountry", typeof(string));
            dt.Columns.Add("certificateORdocumentName", typeof(string));
            dt.Columns.Add("certificateORdocumentNumber", typeof(string));

            foreach (var d in documents)
            {
                dt.Rows.Add(Convert.ToDateTime(d.Certificateordocumentexpirydate), Convert.ToDateTime(d.Certificateordocumentissuedate),
                    d.Certificateordocumentissuingcountry, d.Certificateordocumentname, d.Certificateordocumentnumber);
            }
            return dt;
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    private DataTable ToExperienceTable(List<PreviousSeaExperiance> experiences)
    {
        try
        {
            var dt = new DataTable();
            dt.Columns.Add("CompanyName", typeof(string));
            dt.Columns.Add("Duration", typeof(string));
            dt.Columns.Add("DWT", typeof(string));
            dt.Columns.Add("EngineType", typeof(string));
            dt.Columns.Add("FromDate", typeof(DateTime));
            dt.Columns.Add("GT", typeof(string));
            dt.Columns.Add("IAS", typeof(string));
            dt.Columns.Add("KW", typeof(string));
            dt.Columns.Add("Position", typeof(string));
            dt.Columns.Add("ToDate", typeof(DateTime));
            dt.Columns.Add("VesselName", typeof(string));
            dt.Columns.Add("VesselType", typeof(string));

            foreach (var e in experiences)
            {
                dt.Rows.Add(e.Companyname, e.Duration, e.Dwt, e.Enginetype, Convert.ToDateTime(e.Fromdate), e.Gt,
                     e.Ias, e.Kw, e.Position, Convert.ToDateTime(e.Todate), e.Vesselname, e.Vesseltype);
            }
            return dt;
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public async Task<ReturnResult> CreateUserProfileAsync(UserProfileModel userProfile)
    {
        try
        {
            var applicant = userProfile.applicant ?? new UserApplicantPostModel();
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@UserId", userProfile.userId);
            parameters.Add("@Mode", _configuration["StoredProcedureModes:Insert"]);

            // Map all scalar fields
            parameters.Add("@Title", applicant.Title);
            parameters.Add("@FirstName", applicant.Firstname);
            parameters.Add("@MiddleName", applicant.Middlename);
            parameters.Add("@Surname", applicant.Surname);
            parameters.Add("@DateOfBirth", Convert.ToDateTime(applicant.Dateofbirth));
            parameters.Add("@Address", applicant.Address);
            parameters.Add("@City", applicant.City);
            parameters.Add("@State", applicant.State);
            parameters.Add("@PostalCode", applicant.Postalcode);
            parameters.Add("@Country", applicant.Country);
            parameters.Add("@CountryCode", applicant.Countrycode);
            parameters.Add("@SecondaryCountryCode", applicant.Secondarycountrycode);
            parameters.Add("@SecondaryNumber", applicant.Secondarynumber);
            parameters.Add("@MobileNumber", applicant.Contactnumber);
            parameters.Add("@WhatsappCountryCode", applicant.Whatsappcountrycode);
            parameters.Add("@WhatsappNumber", applicant.WhatsAppnumber);
            parameters.Add("@SkypeID", applicant.Skypeid);
            parameters.Add("@INDOSnumber", applicant.Indosnumber);
            parameters.Add("@SID", applicant.Sid);
            parameters.Add("@SeamanBookNumber", applicant.Seamanbooknumber);
            parameters.Add("@EducationQualifications", applicant.Educationqualifications);
            parameters.Add("@PersonalStatement", applicant.Personalstatement);
            parameters.Add("@PositionApplied", applicant.Positionapplied);
            parameters.Add("@SalaryExpected", applicant.salaryexpected);
            parameters.Add("@Nationality", applicant.Nationality);
            parameters.Add("@Designation", applicant.Designation);
            parameters.Add("@DateOfAvailability", Convert.ToDateTime(applicant.Dateofavailability));
            parameters.Add("@CompanyId", applicant.Companyid);
            parameters.Add("@CompanyDetails", applicant.Companydetails);
            parameters.Add("@CityID", applicant.Cityid);
            parameters.Add("@StateID", applicant.Stateid);
            parameters.Add("@CountryID", applicant.Countryid);

            // Table-Valued Parameters
            parameters.Add("@Documents", ToDocumentsTable(userProfile.Documents)
                .AsTableValuedParameter("DocumentsTVP"));

            parameters.Add("@Experiences", ToExperienceTable(userProfile.Sea_experience)
                .AsTableValuedParameter("SeaExperienceTVP"));

            // Output
            parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
            parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

            // Execute
            var result = await _dbHelper.QueryAsyncTrans<object>("Insert_ProfileDetails", parameters, _dbKey);

            return new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in CreateUserProfileAsync");
            throw;
        }
    }

    public async Task<ReturnResult> AddUserFilesAsync(UserFilesModel model)
    {

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", model.Userid);
            parameters.Add("@ResumeFile", model.ResumeFile, DbType.Binary);
            parameters.Add("@ImageFile", model.ImageFile, DbType.Binary);
            parameters.Add("@ImageUrl", model.ImageUrl, DbType.String, size: 500);

            parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            var result = await _dbHelper.ExecuteScalarAsync("usp_update_user_files", parameters, _dbKey);

            return new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }
        catch(Exception ex)
        {
            throw;
        }
    }


    public async Task<ReturnResult<UserProfileViewModel>> GetSeafarerProfileAsync(int userId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var dbResult = await _dbHelper.QueryMultipleAsync("usp_get_seafarer_Profile", parameters, _dbKey);

            if (dbResult == null || dbResult.Data.Count < 3)
            {
                return new ReturnResult<UserProfileViewModel>("error", ErrorCodes.InternalServerError, null);
            }

            // First result set: User basic info
            var userRaw = dbResult.Data[0].FirstOrDefault();
            var userProfile = userRaw != null
                ? JsonConvert.DeserializeObject<UserProfileViewModel>(JsonConvert.SerializeObject(userRaw))
                : new UserProfileViewModel();

            // Second result set: Sea experience list
            var experiences = JsonConvert.DeserializeObject<List<SeaExperianceViewPatchModel>>(
                JsonConvert.SerializeObject(dbResult.Data[1])
            );

            // Third result set: Certificates list
            var certificates = JsonConvert.DeserializeObject<List<CertificatesViewPatchModel>>(
                JsonConvert.SerializeObject(dbResult.Data[2])
            );

            userProfile.Experiances = experiences;
            userProfile.Certificates = certificates;

            return new ReturnResult<UserProfileViewModel>(
                dbResult.ReturnStatus ?? "success",
                dbResult.ErrorCode ?? ErrorCodes.Success,
                userProfile
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetSeafarerProfileAsync");
            throw;
        }
    }

    public async Task<ReturnResult<string>> ReferFriendAsync(ReferAFriendModel model)
    {
        var result = new ReturnResult<string>();

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", model.UserId);
            parameters.Add("@FriendEmail", model.FriendEmail);
            parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await _dbHelper.ExecuteScalarAsync("usp_insert_refer_a_friend", parameters, _dbKey);

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

    public async Task<ReturnResult<CompanyViewModel>> fn_GetcompanyProfileAsync(int companyId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            parameters.Add("@ErrorCode", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var result = await _dbHelper.QueryAsyncTrans<CompanyViewModel>("usp_Get_CompanyProfileById", parameters, _dbKey);

            return new ReturnResult<CompanyViewModel>
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode"),
                Data = result.Data?.FirstOrDefault()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in fn_GetcompanyProfileAsync");
            throw;
        }
    }

    public async Task<ReturnResult> UserVideoResume(VideoResumeFilesModel model)
    {

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", model.Userid);
            parameters.Add("@ResumeFile", model.VideoResumeFile, DbType.Binary);

            parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            var result = await _dbHelper.ExecuteScalarAsync("usp_update_user_video_resume", parameters, _dbKey);

            return new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }


    public async Task<ReturnResult> UpdateSeaExperiance(SeaExperianceViewPatchModel experiance)
    {
        try
        {
            var parameters = new DynamicParameters();

            // Adding parameters to the query
            parameters.Add("@Id", experiance.ExperianceId);
            parameters.Add("@Company", experiance.CompanyName);
            parameters.Add("@Duration", experiance.Duration);
            parameters.Add("@DWT", experiance.DWT);
            parameters.Add("@EngineType", experiance.EngineType);
            parameters.Add("@FromDate", (object)experiance.FromDate ?? null);
            parameters.Add("@GT", experiance.GT);
            parameters.Add("@IAS", experiance.IAS);
            parameters.Add("@KW", experiance.KW);
            parameters.Add("@Position", experiance.Position);
            parameters.Add("@ToDate", (object)experiance.ToDate ?? null);
            parameters.Add("@VesselName", experiance.VesselName);
            parameters.Add("@VesselType", experiance.VesselType);

            // Execute the stored procedure
            await _dbHelper.ExecuteScalarAsync("usp_Update_Previous_Sea_Experience", parameters, _dbKey);

            // Return the result
            return new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating/updating the sea experience record");
            throw;
        }
    }

    public async Task<ReturnResult> UpdateCertificatesAsync(CertificatesViewPatchModel certificate)
    {
        try
        {
            var parameters = new DynamicParameters();

            // Adding parameters to the query
            parameters.Add("@Id", certificate.certificateId);
            parameters.Add("@certificateORdocumentName", certificate.CertificateName);
            parameters.Add("@certificateORdocumentNumber", certificate.DocumentNumber);
            parameters.Add("@certificateORdocumentIssuingCountry", certificate.IssuedCountry);
            parameters.Add("@certificateORdocumentExpiryDate", (object)certificate.ExpiryDate ?? null);
            parameters.Add("@certificateORdocumentIssueDate", (object)certificate.IssuedDate ?? null);

            // Execute the stored procedure
            await _dbHelper.ExecuteScalarAsync("usp_update_document_certification", parameters, _dbKey);

            // Return the result
            return new ReturnResult
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating/updating the sea experience record");
            throw;
        }
    }




    public async Task<ReturnResult<VideoResumeFilesModel>> GetVideoResumeAsync(int userId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var dbResult = await _dbHelper.QueryAsyncTrans<VideoResumeFilesModel>(
                "usp_get_user_video_resume", parameters, _dbKey);

            var data = dbResult.Data?.FirstOrDefault();

            // If no row, return a clean not-found response
            var status = dbResult.ReturnStatus ?? (data is null ? "not_found" : "success");
            var code = dbResult.ErrorCode ?? (data is null ? "ERR404" : ErrorCodes.Success);

            return new ReturnResult<VideoResumeFilesModel>(status, code, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetVideoResumeAsync");
            throw;
        }
    }



    public async Task<ReturnResult<UserFilesModel>> GetUserFiles(int userId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var dbResult = await _dbHelper.QueryAsyncTrans<UserFilesModel>(
                "usp_get_user_files", parameters, _dbKey);

            var data = dbResult.Data?.FirstOrDefault();

            // If no row, return a clean not-found response
            var status = dbResult.ReturnStatus ?? (data is null ? "not_found" : "success");
            var code = dbResult.ErrorCode ?? (data is null ? "ERR404" : ErrorCodes.Success);

            return new ReturnResult<UserFilesModel>(status, code, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetUSerFiles");
            throw;
        }
    }

    public async Task<object> CreateNationalityAsync(NationalityModel nationality)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NationalityId", nationality.NationalityId);
            parameters.Add("@Nationality", nationality.Nationality);
            parameters.Add("@CountryCode", nationality.CountryCode);
            parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
            parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

            string storedProcedure = nationality.NationalityId > 0
                ? _configuration["StoredProcedureModes:Update"]
                : _configuration["StoredProcedureModes:Insert"];

            parameters.Add("@Mode", storedProcedure);

            var result = await _dbHelper.QueryAsyncTrans<object>("Nationality_Create", parameters, _dbKey);

            return new
            {
                ReturnStatus = parameters.Get<string>("@ReturnStatus"),
                ErrorCode = parameters.Get<string>("@ErrorCode")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating the Nationality");
            throw;
        }
    }


}



//using System.Data;
//using Dapper;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using ShipJobPortal.Application.DTOs;
//using ShipJobPortal.Domain.Entities;
//using ShipJobPortal.Domain.Interfaces;
//using ShipJobPortal.Infrastructure.DataAccessLayer;
//using ShipJobPortal.Infrastructure.DataHelpers;
//using ShipJobPortal.Infrastructure.Helpers;

//namespace ShipJobPortal.Infrastructure.Repositories
//{
//    public class ProfileRepository : IProfileRepository
//    {
//        private readonly DataAccess _dbHelper;//DataAccess_Improved _dbHelper;
//        private readonly IConfiguration _configuration;
//        private readonly IEncryptionService _encryptionService;
//        private readonly ILogger<ProfileRepository> _logger;

//        public ProfileRepository(IConfiguration configuration, DataAccess dbHelper, IEncryptionService encryptionService, ILogger<ProfileRepository> logger)
//        {
//            _dbHelper = dbHelper;
//            _configuration = configuration;
//            _encryptionService = encryptionService;
//            _logger = logger;
//        }

//        private DataTable ToDocumentsTable(List<DocumentsAndCertications> documents)
//        {
//            var dt = new DataTable();
//            dt.Columns.Add("certificateORdocumentIssueDate", typeof(DateTime));
//            dt.Columns.Add("certificateORdocumentExpiryDate", typeof(DateTime));
//            dt.Columns.Add("certificateORdocumentIssuingCountry", typeof(string));
//            dt.Columns.Add("certificateORdocumentName", typeof(string));
//            dt.Columns.Add("certificateORdocumentNumber", typeof(string));



//            foreach (var d in documents)
//            {
//                dt.Rows.Add(d.certificateORdocumentIssueDate, d.certificateORdocumentExpiryDate,
//                    d.certificateORdocumentIssuingCountry,d.certificateORdocumentName, d.certificateORdocumentNumber);
//            }
//            return dt;

//        }

//        private DataTable ToExperienceTable(List<PreviousSeaExperiance> experiences)
//        {
//            var dt = new DataTable();
//            dt.Columns.Add("CompanyName", typeof(string));
//            dt.Columns.Add("Duration", typeof(string));
//            dt.Columns.Add("DWT", typeof(string));
//            dt.Columns.Add("EngineType", typeof(string));
//            dt.Columns.Add("FromDate", typeof(DateTime));
//            dt.Columns.Add("GT", typeof(string));
//            dt.Columns.Add("IAS", typeof(string));
//            dt.Columns.Add("KW", typeof(string));
//            dt.Columns.Add("Position", typeof(string));
//            dt.Columns.Add("ToDate", typeof(DateTime));
//            dt.Columns.Add("VesselName", typeof(string));
//            dt.Columns.Add("VesselType", typeof(string));

//            foreach (var e in experiences)
//            {
//                dt.Rows.Add(e.CompanyName, e.Duration, e.DWT, e.EngineType, e.FromDate, e.GT,
//                     e.IAS, e.KW, e.Position, e.ToDate, e.VesselName, e.VesselType);
//            }
//            return dt;
//        }

//        public async Task<ReturnResult> CreateUserProfileAsync(UserProfileModel userProfile)
//        {
//            try
//            {
//                DynamicParameters parameters = new DynamicParameters();

//                parameters.Add("@UserDetailId", userProfile.UserDetailId);
//                parameters.Add("@Title", _encryptionService.Encrypt(userProfile.Title));//
//                parameters.Add("@FirstName", _encryptionService.Encrypt(userProfile.FirstName));//
//                parameters.Add("@MiddleName", _encryptionService.Encrypt(userProfile.MiddleName));//
//                parameters.Add("@Surname", _encryptionService.Encrypt(userProfile.Surname));//
//                parameters.Add("@DateOfBirth", userProfile.DateOfBirth);//
//                parameters.Add("@Nationality", userProfile.Nationality);//
//                parameters.Add("@CountryCode", userProfile.CountryCode);//
//                parameters.Add("@MobileNumber", _encryptionService.Encrypt(userProfile.MobileNumber));//
//                parameters.Add("@SecondaryCountryCode", _encryptionService.Encrypt(userProfile.SecondaryCountryCode));//
//                parameters.Add("@SecondaryNumber", _encryptionService.Encrypt(userProfile.SecondaryNumber));//
//                parameters.Add("@WhatsappCountryCode", _encryptionService.Encrypt(userProfile.WhatsappCountryCode));//
//                parameters.Add("@WhatsappNumber", _encryptionService.Encrypt(userProfile.WhatsappNumber));//
//                parameters.Add("@SkypeID", _encryptionService.Encrypt(userProfile.SkypeID));//
//                parameters.Add("@Email", _encryptionService.Encrypt(userProfile.Email));//
//                parameters.Add("@Address", _encryptionService.Encrypt(userProfile.Address));//
//                parameters.Add("@AddressCity", _encryptionService.Encrypt(userProfile.AddressCity));//
//                parameters.Add("@AddressStateORProvince", _encryptionService.Encrypt(userProfile.AddressStateORProvince));//
//                parameters.Add("@AddressPostalCode", _encryptionService.Encrypt(userProfile.AddressPostalCode));//
//                parameters.Add("@AddressCountry", _encryptionService.Encrypt(userProfile.AddressCountry));//
//                parameters.Add("@EducationDetails", _encryptionService.Encrypt(userProfile.PositionAppliedFor));//
//                parameters.Add("@SalaryExpectations", _encryptionService.Encrypt(userProfile.SalaryExpectations));//
//                parameters.Add("@PersonalStatement", _encryptionService.Encrypt(userProfile.PersonalStatement));//
//                parameters.Add("@INDOSnumber", _encryptionService.Encrypt(userProfile.INDOSnumber));//
//                parameters.Add("@SID", _encryptionService.Encrypt(userProfile.SID));//
//                parameters.Add("@SeamanBookNumber", _encryptionService.Encrypt(userProfile.SeamanBookNumber));//
//                parameters.Add("@Image", userProfile.Image, DbType.Binary);
//                parameters.Add("@UserId", userProfile.UserId);//

//                // TVPs
//                parameters.Add("@Documents", ToDocumentsTable(userProfile.Documents)
//                    .AsTableValuedParameter("DocumentsTVP"));

//                parameters.Add("@Experiences", ToExperienceTable(userProfile.Experiences)
//                    .AsTableValuedParameter("SeaExperienceTVP"));

//                // Execute SP
//                string storedProcedure = userProfile.UserDetailId > 0 ? _configuration["StoredProcedureModes:Update"] : _configuration["StoredProcedureModes:Insert"];

//                parameters.Add("@Mode", storedProcedure);

//                parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
//                parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

//                var result = await _dbHelper.QueryAsyncTrans("Insert_ProfileDetails", parameters);

//                return new ReturnResult
//                {
//                    ReturnStatus = parameters.Get<string>("@ReturnStatus"),
//                    ErrorCode = parameters.Get<string>("@ErrorCode")
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Exception occurred in CreateUserProfileAsync");
//                throw;
//            }

//        }

//        public async Task<ReturnResult> AddUserFilesAsync(UserFilesModel userFiles)
//        {
//            try
//            {
//                var parameters = new DynamicParameters();
//                parameters.Add("@UserId", userFiles.Userid);
//                parameters.Add("@ResumeFile", userFiles.ResumeFile);
//                parameters.Add("@ImageFile", userFiles.ImageFile);
//                parameters.Add("@ImageUrl", userFiles.ImageUrl);

//                parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
//                parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

//                await _dbHelper.QueryAsyncTrans("usp_update_user_files", parameters);

//                return new ReturnResult
//                {
//                    ReturnStatus = parameters.Get<string>("@ReturnStatus"),
//                    ErrorCode = parameters.Get<string>("@ErrorCode")
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Exception in AddUserFilesAsync");
//                throw;
//            }
//        }


//        public async Task<object> CreateNationalityAsync(NationalityModel nationality)
//        {
//            try
//            {
//                var parameters = new DynamicParameters();
//                parameters.Add("@NationalityId", nationality.NationalityId);
//                parameters.Add("@Nationality", nationality.Nationality);
//                parameters.Add("@CountryCode", nationality.CountryCode);
//                parameters.Add("@ReturnStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);
//                parameters.Add("@ErrorCode", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

//                // ✅ Required output
//                // parameters.Add("@ReturnNationalityId", dbType: DbType.Int32, direction: ParameterDirection.Output);


//                string storedProcedure = nationality.NationalityId > 0 ? _configuration["StoredProcedureModes:Update"] : _configuration["StoredProcedureModes:Insert"];

//                parameters.Add("@Mode", storedProcedure);

//                var result = await _dbHelper.QueryAsyncTrans("Nationality_Create", parameters);

//                var returnStatus = parameters.Get<string>("@ReturnStatus");
//                var errorCode = parameters.Get<string>("@ErrorCode");

//                return new
//                {
//                    ReturnStatus = returnStatus,
//                    ErrorCode = errorCode
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while creating the Nationality");
//                throw;
//            }
//        }


//    }
//}