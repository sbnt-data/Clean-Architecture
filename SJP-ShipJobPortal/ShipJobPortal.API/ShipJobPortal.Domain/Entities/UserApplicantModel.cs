namespace ShipJobPortal.Domain.Entities;

public class UserApplicantModel
{
    public string Role { get; set; }
    public string loginType { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string Surname { get; set; }
    public string Title { get; set; }
    public string Designation { get; set; }
    public int CompanyId { get; set; }
    public string CompanyDetails { get; set; }
    //public string Website { get; set; }
    public string Email { get; set; }
    public string ContactNumber { get; set; }
    public string CountryCode { get; set; }
    public string Nationality { get; set; }
    public string Address { get; set; }
    public int CityID { get; set; }
    public int StateID { get; set; }
    public int CountryID { get; set; }
    public string PostalCode { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Password { get; set; }
}
public class UserProfileModel
{
    public int userId { get; set; }
    public UserApplicantPostModel? applicant { get; set; }
    public List<DocumentsAndCertications>? Documents { get; set; }
    public List<PreviousSeaExperiance>? Sea_experience { get; set; }
}
public class UserApplicantPostModel
{
    public string? Title { get; set; }
    public string? Surname { get; set; }
    public string? Firstname { get; set; }
    public string? Middlename { get; set; }
    public string? Countrycode { get; set; }
    public string? Contactnumber { get; set; }
    public int? Companyid { get; set; }
    public string? Companydetails { get; set; }
    public string? Whatsappcountrycode { get; set; }
    public string? WhatsAppnumber { get; set; }
    public string? Secondarycountrycode { get; set; }
    public string? Secondarynumber { get; set; }
    public string? Email { get; set; }
    public string? Nationality { get; set; }
    public string? Designation { get; set; }
    public string? Positionapplied { get; set; }
    public string? Dateofavailability { get; set; }//DateTime
    public string? salaryexpected { get; set; }
    public string? Dateofbirth { get; set; }//DateTime
    public string? Seamanbooknumber { get; set; }
    public string? Skypeid { get; set; }
    public string? Sid { get; set; }
    public string? Indosnumber { get; set; }
    public string? Address { get; set; }
    public string? Cityid { get; set; }
    public string? Countryid { get; set; }
    public string? Stateid { get; set; }
    public string? Postalcode { get; set; }
    public string? Personalstatement { get; set; }
    public string? Educationqualifications { get; set; }
    public string? Logintype { get; set; }
    public DateTime? Createdon { get; set; }
    public int? Createdby { get; set; }
    public DateTime? Updatedon { get; set; }
    public int? Updatedby { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}
public class DocumentsAndCertications
{

    public string? Certificateordocumentexpirydate { get; set; }//DateTime
    public string? Certificateordocumentissuedate { get; set; }//DateTime
    public string? Certificateordocumentissuingcountry { get; set; }
    public string? Certificateordocumentname { get; set; }
    public string? Certificateordocumentnumber { get; set; }
}
public class PreviousSeaExperiance
{
    public string? Companyname { get; set; }
    public string? Duration { get; set; }
    public string? Dwt { get; set; }
    public string? Enginetype { get; set; }
    public string? Fromdate { get; set; }//DateTime
    public string? Gt { get; set; }
    public string? Ias { get; set; }
    public string? Kw { get; set; }
    public string? Position { get; set; }
    public string? Todate { get; set; }//DateTime
    public string? Vesselname { get; set; }
    public string? Vesseltype { get; set; }
}

public class UserFilesModel
{
    public int Userid { get; set; }
    public byte[]? ResumeFile {  get; set; }
    public byte[]? ImageFile { get; set; }
    public string? ImageUrl { get; set; }

}

public class VideoResumeFilesModel
{
    public int? Userid { get; set; }
    public byte[]? VideoResumeFile { get; set; }

}

public class UserProfileViewModel
{
    public int Userid { get; set; }
    public string? Name {  get; set; }
    public string? Rank { get; set; }
    public string? Nationality { get; set; }
    public int? Age { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public string? DateofAvailability { get; set; }
    public string? Location { get; set; }
    public string? TotalSeaTime { get; set; }
    public string? LastVessel { get; set; }
    public string? Rating { get; set; }
    public List<SeaExperianceViewPatchModel> ?Experiances { get; set; }
    public List<CertificatesViewPatchModel> ?Certificates { get; set; }
}
public class SeaExperianceViewPatchModel
{
    public int? ExperianceId { get; set; }
    public string? VesselName { get; set; }
    public string? VesselType { get; set; }
    public string? Rank { get; set; }
    public string? CompanyName { get; set; }
    public string? Duration { get; set; }
    public string? DWT {  get; set; }
    public string? Period { get; set; }
    public string? Position { get; set; }
    public string? Route { get; set; }
    public string? GT { get; set; }
    public string? FromDate {  get; set; }
    public string? EngineType {  get; set; }
    public string? IAS {  get; set; }
    public string? KW {  get; set; }
    public string? ToDate {  get; set; }
}
public class CertificatesViewPatchModel
{
    public int? certificateId { get; set; }
    public string? CertificateName { get; set; }
    public string? IssuedDate { get; set; }
    public string? IssuedCountry { get; set; }
    public string? ExpiryDate { get; set; }
    public string? Status { get; set; }
    public string? DocumentNumber { get; set; }
}

public class ExistingUserData
{
    public int? UserId { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? RefreshToken { get; set; }
    public string? RefreshTokenExpiry { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash {  get; set; }
    public string? ResumeStatus {  get; set; }
}

public class ModulePrevilleagesModel
{
    public string? RoleName { get; set; }
    public string? ModuleID { get; set; }
    public string? ModuleName { get; set; }
    public string? ModulePath { get; set; }
    public int? SortOrder { get; set; }
    public int? ViewAllowed { get; set; }
    public int? AddAllowed { get; set; }
    public int? EditAllowed { get; set; }
    public int? DeleteAllowed { get; set; }
    public int? PrintAllowed { get; set; }
    public int? EmailAllowed { get; set; }
    public int? Hide { get; set; }
    public int? ModuleIsActive { get; set; }
    public int? MappingIsActive { get; set; }
}

public class UserCreationResult
{
    public string Status { get; set; }
    public int UserId { get; set; }
}


public class ReferAFriendModel
{
    public int UserId { get; set; }
    public string FriendEmail { get; set; }
}

public class CompanyViewModel
{
    public int? companyid { get; set; }
    public string? companyname { get; set; }
    public string? companyaddress { get; set; }
    public string? email { get; set; }
    public string? contactnumber { get; set; }
    public string? city { get; set; }
    public string? state { get; set; }
    public string? country { get; set; }
    public string? postalcode { get; set; }
    public string? website { get; set; }
}

public class ResetPasswordDataModel
{
    public string? UserID { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
}