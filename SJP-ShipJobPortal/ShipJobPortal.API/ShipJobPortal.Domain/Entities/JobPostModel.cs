using System.Text.Json.Serialization;

namespace ShipJobPortal.Domain.Entities;

public class JobPostModel
{
    public int JobId { get; set; }
    public int CompanyId { get; set; }//
    public int UserId { get; set; }//
    public string JobTitle { get; set; }//
    public int? NoVacancy { get; set; }//
    public int NationalityId { get; set; }//
    public string? JobDescription { get; set; }//
    public DateTime ?OpenDate { get; set; }//
    public DateTime? CloseDate { get; set; }//
    public int? MinAge { get; set; }//
    public int? MaxAge { get; set; }//
    public string? SendNotification { get; set; }
    public string Salary { get; set; }//
    public string PublishedDate { get; set; }
    public int vesselTypeId { get; set; }
    public int PositionId { get; set; }//
    public int LocationId { get; set; }//
    public int PreferedLocationId { get; set; }//
    public int DurationId { get; set; }//
    public DateTime? UpdatedOn { get; set; }
    [JsonIgnore]
    public string Status { get; set; }
}
public class JobViewFilteredModel
{
    public PaginationModel Pagination { get; set; }
    public List<JobViewModel> JobsList { get; set; }
}

public class PaginationModel
{
    public string? TotalItems { get; set; }
    public string? TotalPages { get; set; }
    public string? CurrentPage { get; set; }
    public string? ItemsPerPage { get; set; }
}

public class JobViewModel
{
    public int JobId { get; set; }
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public int NoVacancy { get; set; }
    public int NationalityId { get; set; }
    public string? Nationality { get; set; }
    public string? JobDescription { get; set; }
    public string? OpenDate { get; set; }
    public string? CloseDate { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public string? PublishedDate { get; set; }
    public string? Salary { get; set; }
    public int? PositionId { get; set; }
    public string? Position { get; set; }
    public int? DurationId { get; set; }
    public string? Duration { get; set; }
    public string? SendNotification { get; set; }
    public int? VesselTypeId { get; set; }
    public string? vesselType { get; set; }
    public int? LocationId { get; set; }
    public string? LocationName { get; set; }
    public int? PreferedLocationId { get; set; }
    public string? PreferedLocationName { get; set; }
    public string? IsSaved { get; set; }
    public string? appliedcount { get; set; }
    public string? IsApplied { get; set; }
    public string? viewcount {  get; set; }
    [JsonIgnore]
    public string? Status { get; set; }
}

public class JobViewCountModel
{
    public int userid { get; set; }
    public int jobid { get; set; }
}
//public class jobUserView
//{
//    public int VacancyId { get; set; }
//    public int CompanyId { get; set; }
//    public int UserId { get; set; }
//    public string JobTitle { get; set; }
//    public int? NoVacancy { get; set; }
//    public int NationalityId { get; set; }
//    public string? JobDescription { get; set; }
//    public DateTime OpenDate { get; set; }
//    public DateTime? CloseDate { get; set; }
//    public int? MinAge { get; set; }
//    public int? MaxAge { get; set; }
//    public string? SendNotification { get; set; }
//    public string Salary { get; set; }
//    public string PublishedDate { get; set; }
//    public string? vesselType { get; set; }
//    public string? VesselTypeName { get; set; }

//    public string? Position { get; set; }
//    public string? PositionName { get; set; }

//    public string? Location { get; set; }
//    public string? Duration { get; set; }
//    public string? DurationName { get; set; }
//    [JsonIgnore]
//    public string Status { get; set; }
//}