using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Application.DTOs
{
    public class JobCreateDto
    {
        public int JobId { get; set; }
        public int CompanyId { get; set; }//
        public int UserId { get; set; }//
        public string JobTitle { get; set; }//
        public int? NoVacancy { get; set; }//
        public int NationalityId { get; set; }//
        public string? JobDescription { get; set; }//
        public DateTime OpenDate { get; set; }//
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
    }


    public class JobViewFilteredDto
    {
        public PaginationDto Pagination { get; set; }
        public List<jobViewDto> JobsList { get; set; }
    }

    public class PaginationDto
    {
        public string? TotalItems { get; set; }
        public string? TotalPages { get; set; }
        public string? CurrentPage { get; set; }
        public string? ItemsPerPage { get; set; }
    }
    public class jobViewDto
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
        public string? appliedcount {  get; set; }
        public string? IsApplied { get; set; }
        public string? viewcount { get; set; }


    }

    public class JobPatchDto
    {
        public string? JobTitle { get; set; }
        public string? Salary { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public int? NoVacancy { get; set; }
        public int? NationalityId { get; set; }
        public string? JobDescription { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? SendNotification { get; set; }
        public int? VesselType { get; set; }
        public int? Position { get; set; }
        public int? PreferedLocation { get; set; }
        public int? Duration { get; set; }
        public string? PublishedDate { get; set; }
    }

    public class JobViewCountDto
    {
        public int userid { get; set; }
        public int jobid { get; set; }
    }
    //public class jobViewDto
    //{
    //    public int VacancyId { get; set; }
    //    public int CompanyId { get; set; }
    //    public string? JobTitle { get; set; }
    //    public string? Company { get; set; }
    //    public int NoVacancy { get; set; }
    //    public int NationalityId { get; set; }
    //    public string? Nationality { get; set; }
    //    public string? JobDescription { get; set; }
    //    public string? OpenDate { get; set; }
    //    public string? CloseDate { get; set; }
    //    public int MinAge { get; set; }
    //    public int MaxAge { get; set; }
    //    public string? PublishedDate { get; set; }
    //    public string? Salary { get; set; }
    //    public string? VesselType { get; set; }
    //    public string? Position { get; set; }
    //    public string? Duration { get; set; }
    //}
}